'use client';

import { use, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import Link from 'next/link';
import { buttonVariants } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { PageHeader } from '@/components/common/PageHeader';
import { LoadingSpinner } from '@/components/common/LoadingSpinner';
import { ErrorMessage } from '@/components/common/ErrorMessage';
import { Pagination } from '@/components/common/Pagination';
import { ItemPedidoCard } from '@/components/pedidos/ItemPedidoCard';
import { ProdutoCard } from '@/components/produtos/ProdutoCard';
import { useProdutos } from '@/hooks/useProdutos';
import { obterPedido } from '@/services/pedidosService';
import { formatarMoeda } from '@/lib/formatters';

export default function PedidoPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const pedidoId = Number(id);
  const [paginaProdutos, setPaginaProdutos] = useState(1);

  const { data: pedido, isLoading, isError, error } = useQuery({
    queryKey: ['pedido', pedidoId],
    queryFn: () => obterPedido(pedidoId),
    refetchOnMount: true,
  });

  const { data: produtos, isLoading: loadingProdutos } = useProdutos({
    pagina: paginaProdutos,
    itensPorPagina: 12,
  });

  if (isLoading) return <LoadingSpinner />;
  if (isError) return <ErrorMessage message={(error as Error).message} />;

  return (
    <div>
      <PageHeader
        title={`Pedido #${pedidoId}`}
        action={
          <Link href="/" className={cn(buttonVariants({ variant: 'outline' }))}>Voltar</Link>
        }
      />

      {pedido && (
        <div className="mb-6 px-5 py-4 border border-border border-l-[3px] border-l-primary rounded-xl bg-card">
          <p className="text-2xl font-bold text-primary tracking-tight">{formatarMoeda(pedido.valorTotal)}</p>
          <p className="text-sm text-muted-foreground mt-1">{pedido.cliente?.nome}</p>
        </div>
      )}

      {pedido && pedido.itens && pedido.itens.length > 0 && (
        <div className="mb-8">
          <h2 className="text-base font-semibold text-foreground mb-3">Itens do Pedido</h2>
          <div className="space-y-2">
            {pedido.itens.map((item) => (
              <ItemPedidoCard key={item.produto.codProduto} item={item} pedidoId={pedidoId} />
            ))}
          </div>
        </div>
      )}

      <div>
        <h2 className="text-base font-semibold text-foreground mb-3">Produtos Disponíveis</h2>
        {loadingProdutos ? (
          <LoadingSpinner />
        ) : (
          <>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {produtos?.items?.map((produto) => (
                <ProdutoCard key={produto.codProduto} produto={produto} pedidoId={pedidoId} />
              ))}
            </div>
            {produtos && (
              <Pagination
                paginaAtual={produtos.paginaAtual}
                totalPaginas={produtos.totalPaginas}
                totalItens={produtos.totalItens}
                onPageChange={setPaginaProdutos}
              />
            )}
          </>
        )}
      </div>
    </div>
  );
}
