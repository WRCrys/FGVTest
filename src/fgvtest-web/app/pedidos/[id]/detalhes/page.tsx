import Link from 'next/link';
import { buttonVariants } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { PageHeader } from '@/components/common/PageHeader';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { obterPedido } from '@/services/pedidosService';
import { formatarCNPJ, formatarMoeda, formatarData } from '@/lib/formatters';

export default async function DetalhesPedidoPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const pedido = await obterPedido(Number(id));

  return (
    <div>
      <PageHeader
        title={`Detalhes do Pedido #${pedido.codPedido}`}
        action={
          <Link href={`/pedidos/${pedido.codPedido}`} className={cn(buttonVariants({ variant: 'outline' }))}>Voltar</Link>
        }
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <div className="p-5 border border-border rounded-xl space-y-2 bg-card">
          <h2 className="font-semibold text-sm text-muted-foreground uppercase tracking-wider mb-3">Pedido</h2>
          <p className="text-sm"><span className="font-medium text-foreground">Número:</span> <span className="font-mono text-muted-foreground">#{pedido.codPedido}</span></p>
          <p className="text-sm"><span className="font-medium text-foreground">Data:</span> <span className="text-muted-foreground">{formatarData(pedido.dataPedido)}</span></p>
          <p className="text-sm"><span className="font-medium text-foreground">Total:</span> <span className="text-primary font-bold">{formatarMoeda(pedido.valorTotal)}</span></p>
        </div>
        <div className="p-5 border border-border rounded-xl space-y-2 bg-card">
          <h2 className="font-semibold text-sm text-muted-foreground uppercase tracking-wider mb-3">Cliente</h2>
          <p className="text-sm"><span className="font-medium text-foreground">Nome:</span> <span className="text-muted-foreground">{pedido.cliente?.nome}</span></p>
          <p className="text-sm"><span className="font-medium text-foreground">CNPJ:</span> <span className="font-mono text-muted-foreground">{pedido.cliente?.cnpj ? formatarCNPJ(pedido.cliente.cnpj) : '—'}</span></p>
          <p className="text-sm"><span className="font-medium text-foreground">E-mail:</span> <span className="text-muted-foreground">{pedido.cliente?.email}</span></p>
        </div>
      </div>

      <div>
        <h2 className="font-semibold text-base text-foreground mb-3">Itens do Pedido</h2>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Produto</TableHead>
              <TableHead className="text-right">Quantidade</TableHead>
              <TableHead className="text-right">Preço Unitário</TableHead>
              <TableHead className="text-right">Subtotal</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {pedido.itens?.map((item) => (
              <TableRow key={item.produto.codProduto}>
                <TableCell>{item.produto.nome}</TableCell>
                <TableCell className="text-right">{item.quantidade}</TableCell>
                <TableCell className="text-right">{formatarMoeda(item.precoUnitario)}</TableCell>
                <TableCell className="text-right">{formatarMoeda(item.quantidade * item.precoUnitario)}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
