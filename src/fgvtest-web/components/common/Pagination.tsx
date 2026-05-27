'use client';

import { Button } from '@/components/ui/button';
import { ChevronLeft, ChevronRight } from 'lucide-react';

interface PaginationProps {
  paginaAtual: number;
  totalPaginas: number;
  totalItens: number;
  onPageChange: (pagina: number) => void;
}

export function Pagination({ paginaAtual, totalPaginas, totalItens, onPageChange }: PaginationProps) {
  return (
    <div className="flex items-center justify-between mt-6 pt-4 border-t border-border">
      <span className="text-sm text-muted-foreground font-mono">
        {totalItens} {totalItens === 1 ? 'registro' : 'registros'}
      </span>
      <div className="flex items-center gap-1">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => onPageChange(paginaAtual - 1)}
          disabled={paginaAtual === 1}
          className="h-8 w-8 p-0"
        >
          <ChevronLeft size={16} strokeWidth={2} />
        </Button>
        <span className="text-sm px-3 text-foreground font-medium tabular-nums">
          {paginaAtual} / {totalPaginas}
        </span>
        <Button
          variant="ghost"
          size="sm"
          onClick={() => onPageChange(paginaAtual + 1)}
          disabled={paginaAtual === totalPaginas || totalPaginas === 0}
          className="h-8 w-8 p-0"
        >
          <ChevronRight size={16} strokeWidth={2} />
        </Button>
      </div>
    </div>
  );
}
