'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { PageHeader } from '@/components/common/PageHeader';
import { LoadingSpinner } from '@/components/common/LoadingSpinner';
import { EmptyState } from '@/components/common/EmptyState';
import { ErrorMessage } from '@/components/common/ErrorMessage';
import { Pagination } from '@/components/common/Pagination';
import { PedidoCard } from '@/components/pedidos/PedidoCard';
import { PedidoFiltros } from '@/components/pedidos/PedidoFiltros';
import { CriarPedidoModal } from '@/components/pedidos/CriarPedidoModal';
import { usePedidos } from '@/hooks/usePedidos';

export default function PedidosPage() {
  const [pagina, setPagina] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [dataInicio, setDataInicio] = useState('');
  const [dataFim, setDataFim] = useState('');

  const { data, isLoading, isError, error } = usePedidos({
    pagina,
    itensPorPagina: 12,
    dataInicio: dataInicio || undefined,
    dataFim: dataFim || undefined,
  });

  function handleLimparFiltros() {
    setDataInicio('');
    setDataFim('');
    setPagina(1);
  }

  return (
    <div>
      <PageHeader
        title="Pedidos"
        action={
          <Button onClick={() => setModalOpen(true)}>Novo Pedido</Button>
        }
      />

      <PedidoFiltros
        dataInicio={dataInicio}
        dataFim={dataFim}
        onDataInicioChange={(v) => { setDataInicio(v); setPagina(1); }}
        onDataFimChange={(v) => { setDataFim(v); setPagina(1); }}
        onLimpar={handleLimparFiltros}
      />

      {isLoading && <LoadingSpinner />}
      {isError && <ErrorMessage message={(error as Error).message} />}

      {!isLoading && !isError && (
        <>
          {(!data?.items || data.items.length === 0) ? (
            <EmptyState message="Nenhum pedido encontrado." />
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {data.items.map((pedido) => (
                <PedidoCard key={pedido.codPedido} pedido={pedido} />
              ))}
            </div>
          )}

          {data && data.totalPaginas > 0 && (
            <Pagination
              paginaAtual={data.paginaAtual}
              totalPaginas={data.totalPaginas}
              totalItens={data.totalItens}
              onPageChange={setPagina}
            />
          )}
        </>
      )}

      <CriarPedidoModal open={modalOpen} onOpenChange={setModalOpen} />
    </div>
  );
}
