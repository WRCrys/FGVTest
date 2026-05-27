'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { criarProdutoSchema, CriarProdutoFormData } from '@/schemas/produtoSchema';
import { criarProduto } from '@/services/produtosService';

export function ProdutoForm() {
  const { register, handleSubmit, formState: { errors }, reset } = useForm<CriarProdutoFormData>({
    resolver: zodResolver(criarProdutoSchema),
  });

  const mutation = useMutation({
    mutationFn: criarProduto,
    onSuccess: () => {
      toast.success('Produto cadastrado com sucesso.');
      reset();
    },
    onError: (error: Error) => {
      toast.error('Erro', { description: error.message });
    },
  });

  function onSubmit(data: CriarProdutoFormData) {
    mutation.mutate(data);
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 max-w-md">
      <div className="space-y-1">
        <Label htmlFor="nome">Nome</Label>
        <Input id="nome" {...register('nome')} placeholder="Nome do produto" />
        {errors.nome && <p className="text-sm text-destructive">{errors.nome.message}</p>}
      </div>
      <div className="space-y-1">
        <Label htmlFor="preco">Preço</Label>
        <Input id="preco" type="number" step="0.01" min="0" {...register('preco', { valueAsNumber: true })} placeholder="0,00" />
        {errors.preco && <p className="text-sm text-destructive">{errors.preco.message}</p>}
      </div>
      <div className="space-y-1">
        <Label htmlFor="estoque">Estoque</Label>
        <Input id="estoque" type="number" min="0" {...register('estoque', { valueAsNumber: true })} placeholder="0" />
        {errors.estoque && <p className="text-sm text-destructive">{errors.estoque.message}</p>}
      </div>
      <Button type="submit" disabled={mutation.isPending}>
        {mutation.isPending ? 'Cadastrando...' : 'Cadastrar Produto'}
      </Button>
    </form>
  );
}
