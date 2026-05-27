'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { ItemPedido } from '@/types/pedido';
import { formatarMoeda } from '@/lib/formatters';
import { removerItem } from '@/services/pedidosService';
import { Trash2 } from 'lucide-react';

interface ItemPedidoCardProps {
  item: ItemPedido;
  pedidoId: number;
}

export function ItemPedidoCard({ item, pedidoId }: ItemPedidoCardProps) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () => removerItem(pedidoId, item.produto.codProduto),
    onSuccess: () => {
      toast.success('Item removido com sucesso.');
      queryClient.invalidateQueries({ queryKey: ['pedido', pedidoId] });
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
    },
    onError: (error: Error) => {
      toast.error('Erro', { description: error.message });
    },
  });

  return (
    <div className="flex items-center justify-between px-4 py-3 bg-card border border-border rounded-xl">
      <div>
        <p className="text-sm font-medium text-foreground">{item.produto.nome}</p>
        <p className="text-xs text-muted-foreground mt-0.5 font-mono">
          {item.quantidade} × {formatarMoeda(item.precoUnitario)} ={' '}
          <span className="text-foreground font-semibold">
            {formatarMoeda(item.quantidade * item.precoUnitario)}
          </span>
        </p>
      </div>
      <Button
        size="sm"
        variant="ghost"
        onClick={() => mutation.mutate()}
        disabled={mutation.isPending}
        className="text-muted-foreground hover:text-destructive hover:bg-destructive/8 h-8 w-8 p-0 shrink-0"
      >
        <Trash2 size={14} strokeWidth={1.75} />
      </Button>
    </div>
  );
}
