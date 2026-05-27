import Link from 'next/link';
import { buttonVariants } from '@/components/ui/button';
import { Pedido } from '@/types/pedido';
import { formatarCNPJ, formatarMoeda, formatarData } from '@/lib/formatters';
import { cn } from '@/lib/utils';
import { Calendar, Building2 } from 'lucide-react';

interface PedidoCardProps {
  pedido: Pedido;
}

export function PedidoCard({ pedido }: PedidoCardProps) {
  return (
    <div className="group bg-card border border-border border-l-[3px] border-l-primary rounded-xl p-5 hover:shadow-[0_8px_24px_-6px_rgba(0,0,0,0.1)] hover:-translate-y-0.5 active:translate-y-0 active:scale-[0.99] transition-all duration-200">
      <div className="flex items-start justify-between mb-3">
        <span className="text-xs font-mono font-semibold text-muted-foreground tracking-widest uppercase">
          #{String(pedido.codPedido).padStart(4, '0')}
        </span>
        <span className="flex items-center gap-1 text-xs text-muted-foreground">
          <Calendar size={11} strokeWidth={2} />
          {formatarData(pedido.dataPedido)}
        </span>
      </div>

      <p className="font-semibold text-foreground text-sm leading-tight mb-1 truncate">
        {pedido.cliente?.nome ?? '—'}
      </p>

      {pedido.cliente?.cnpj && (
        <p className="flex items-center gap-1 text-xs text-muted-foreground mb-3">
          <Building2 size={10} strokeWidth={2} />
          {formatarCNPJ(pedido.cliente.cnpj)}
        </p>
      )}

      <p className="text-xl font-bold text-primary tracking-tight mb-4">
        {formatarMoeda(pedido.valorTotal)}
      </p>

      <div className="flex gap-2">
        <Link
          href={`/pedidos/${pedido.codPedido}`}
          className={cn(buttonVariants({ size: 'sm' }), 'flex-1 justify-center')}
        >
          Ver Pedido
        </Link>
        <Link
          href={`/pedidos/${pedido.codPedido}/detalhes`}
          className={cn(buttonVariants({ variant: 'outline', size: 'sm' }))}
        >
          Detalhes
        </Link>
      </div>
    </div>
  );
}
