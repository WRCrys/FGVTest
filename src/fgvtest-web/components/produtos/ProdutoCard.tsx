'use client';

import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Produto } from '@/types/produto';
import { formatarMoeda } from '@/lib/formatters';
import { adicionarItem } from '@/services/pedidosService';

interface ProdutoCardProps {
  produto: Produto;
  pedidoId: number;
}

export function ProdutoCard({ produto, pedidoId }: ProdutoCardProps) {
  const [adicionando, setAdicionando] = useState(false);
  const [quantidade, setQuantidade] = useState(1);
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () => adicionarItem(pedidoId, { codProduto: produto.codProduto, quantidade }),
    onSuccess: () => {
      toast.success('Item adicionado ao pedido.');
      queryClient.invalidateQueries({ queryKey: ['pedido', pedidoId] });
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
      setAdicionando(false);
      setQuantidade(1);
    },
    onError: (error: Error) => {
      toast.error('Erro', { description: error.message });
    },
  });

  const semEstoque = produto.estoque === 0;

  return (
    <div className="bg-card border border-border rounded-xl p-5 flex flex-col gap-3 hover:shadow-sm transition-shadow duration-200">
      <div>
        <p className="font-semibold text-foreground text-sm leading-tight">{produto.nome}</p>
      </div>

      <div className="flex items-center justify-between text-sm">
        <span className="text-xl font-bold text-primary tracking-tight">
          {formatarMoeda(produto.preco)}
        </span>
        {semEstoque ? (
          <span className="inline-flex items-center text-xs font-medium text-destructive bg-destructive/8 px-2 py-0.5 rounded-full">
            Sem estoque
          </span>
        ) : (
          <span className="inline-flex items-center text-xs font-medium text-muted-foreground bg-muted px-2 py-0.5 rounded-full">
            {produto.estoque} un.
          </span>
        )}
      </div>

      {semEstoque ? (
        <Button size="sm" disabled className="w-full">
          Adicionar
        </Button>
      ) : adicionando ? (
        <div className="flex gap-2">
          <Input
            type="number"
            min={1}
            max={produto.estoque}
            value={quantidade}
            onChange={(e) => setQuantidade(Number(e.target.value))}
            className="w-20 h-9 text-sm"
          />
          <Button
            size="sm"
            className="flex-1"
            onClick={() => {
              if (quantidade < 1 || quantidade > produto.estoque) {
                toast.error('Quantidade inválida', { description: `Estoque: ${produto.estoque}` });
                return;
              }
              mutation.mutate();
            }}
            disabled={mutation.isPending}
          >
            {mutation.isPending ? '...' : 'Confirmar'}
          </Button>
          <Button size="sm" variant="ghost" onClick={() => setAdicionando(false)} className="px-2">
            ✕
          </Button>
        </div>
      ) : (
        <Button size="sm" className="w-full" onClick={() => setAdicionando(true)}>
          Adicionar
        </Button>
      )}
    </div>
  );
}
