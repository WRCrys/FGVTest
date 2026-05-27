'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { criarPedidoSchema, CriarPedidoFormData } from '@/schemas/pedidoSchema';
import { criarPedido } from '@/services/pedidosService';
import { limparCNPJ, formatarCNPJ } from '@/lib/formatters';

interface CriarPedidoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CriarPedidoModal({ open, onOpenChange }: CriarPedidoModalProps) {
  const router = useRouter();
  const queryClient = useQueryClient();

  const { handleSubmit, formState: { errors }, reset, setValue, watch } = useForm<CriarPedidoFormData>({
    resolver: zodResolver(criarPedidoSchema),
  });

  const cnpjValue = watch('cnpj', '');

  const mutation = useMutation({
    mutationFn: criarPedido,
    onSuccess: (data) => {
      toast.success('Pedido criado com sucesso.');
      queryClient.invalidateQueries({ queryKey: ['pedidos'] });
      onOpenChange(false);
      reset();
      router.push(`/pedidos/${data.codPedido}`);
    },
    onError: (error: Error) => {
      toast.error('Erro', { description: error.message });
    },
  });

  function handleCnpjChange(e: React.ChangeEvent<HTMLInputElement>) {
    const raw = limparCNPJ(e.target.value);
    const formatted = raw.length <= 14 ? formatarCNPJ(raw) : raw;
    setValue('cnpj', formatted, { shouldValidate: true });
  }

  function onSubmit(formData: CriarPedidoFormData) {
    mutation.mutate({ cnpj: limparCNPJ(formData.cnpj) });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Novo Pedido</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-1">
            <Label htmlFor="cnpj">CNPJ do Cliente</Label>
            <Input
              id="cnpj"
              placeholder="00.000.000/0000-00"
              value={cnpjValue}
              onChange={handleCnpjChange}
            />
            {errors.cnpj && <p className="text-sm text-destructive">{errors.cnpj.message}</p>}
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => { onOpenChange(false); reset(); }}>
              Cancelar
            </Button>
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? 'Criando...' : 'Criar Pedido'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
