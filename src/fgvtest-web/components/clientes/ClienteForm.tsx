'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { criarClienteSchema, CriarClienteFormData } from '@/schemas/clienteSchema';
import { criarCliente } from '@/services/clientesService';
import { limparCNPJ, formatarCNPJ } from '@/lib/formatters';

export function ClienteForm() {
  const { register, handleSubmit, formState: { errors }, reset, setValue, watch } = useForm<CriarClienteFormData>({
    resolver: zodResolver(criarClienteSchema),
    defaultValues: { cnpj: '', nome: '', email: '' },
  });

  const cnpjValue = watch('cnpj', '');

  const mutation = useMutation({
    mutationFn: criarCliente,
    onSuccess: () => {
      toast.success('Cliente cadastrado com sucesso.');
      reset();
    },
    onError: (error: Error) => {
      toast.error('Erro', { description: error.message });
    },
  });

  function handleCnpjChange(e: React.ChangeEvent<HTMLInputElement>) {
    const raw = limparCNPJ(e.target.value);
    if (raw.length <= 14) {
      setValue('cnpj', formatarCNPJ(raw), { shouldValidate: true });
    }
  }

  function onSubmit(data: CriarClienteFormData) {
    mutation.mutate({ ...data, cnpj: limparCNPJ(data.cnpj) });
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 max-w-md">
      <div className="space-y-1">
        <Label htmlFor="cnpj">CNPJ</Label>
        <Input
          id="cnpj"
          placeholder="00.000.000/0000-00"
          value={cnpjValue}
          onChange={handleCnpjChange}
        />
        {errors.cnpj && <p className="text-sm text-destructive">{errors.cnpj.message}</p>}
      </div>
      <div className="space-y-1">
        <Label htmlFor="nome">Nome</Label>
        <Input id="nome" {...register('nome')} placeholder="Razão social" />
        {errors.nome && <p className="text-sm text-destructive">{errors.nome.message}</p>}
      </div>
      <div className="space-y-1">
        <Label htmlFor="email">E-mail</Label>
        <Input id="email" type="email" {...register('email')} placeholder="email@empresa.com" />
        {errors.email && <p className="text-sm text-destructive">{errors.email.message}</p>}
      </div>
      <Button type="submit" disabled={mutation.isPending}>
        {mutation.isPending ? 'Cadastrando...' : 'Cadastrar Cliente'}
      </Button>
    </form>
  );
}
