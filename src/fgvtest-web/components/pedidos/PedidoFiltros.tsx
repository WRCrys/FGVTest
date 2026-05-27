'use client';

import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { X } from 'lucide-react';

interface PedidoFiltrosProps {
  dataInicio: string;
  dataFim: string;
  onDataInicioChange: (v: string) => void;
  onDataFimChange: (v: string) => void;
  onLimpar: () => void;
}

export function PedidoFiltros({ dataInicio, dataFim, onDataInicioChange, onDataFimChange, onLimpar }: PedidoFiltrosProps) {
  const temFiltro = dataInicio || dataFim;

  return (
    <div className="flex flex-wrap items-end gap-3 mb-6 px-4 py-3 bg-muted/50 rounded-xl border border-border">
      <div className="flex flex-col gap-1.5">
        <label className="text-xs font-medium text-muted-foreground uppercase tracking-wider">
          Data início
        </label>
        <Input
          type="date"
          value={dataInicio}
          onChange={(e) => onDataInicioChange(e.target.value)}
          className="h-9 w-40 text-sm bg-card"
        />
      </div>
      <div className="flex flex-col gap-1.5">
        <label className="text-xs font-medium text-muted-foreground uppercase tracking-wider">
          Data fim
        </label>
        <Input
          type="date"
          value={dataFim}
          onChange={(e) => onDataFimChange(e.target.value)}
          className="h-9 w-40 text-sm bg-card"
        />
      </div>
      {temFiltro && (
        <Button
          variant="ghost"
          size="sm"
          onClick={onLimpar}
          className="flex items-center gap-1.5 text-muted-foreground hover:text-foreground h-9"
        >
          <X size={14} strokeWidth={2} />
          Limpar
        </Button>
      )}
    </div>
  );
}
