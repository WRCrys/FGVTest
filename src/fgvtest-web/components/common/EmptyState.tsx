import { Package } from 'lucide-react';

interface EmptyStateProps {
  message?: string;
}

export function EmptyState({ message = 'Nenhum registro encontrado.' }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center py-16 text-center">
      <div className="w-14 h-14 rounded-2xl bg-muted flex items-center justify-center mb-4">
        <Package size={24} className="text-muted-foreground" strokeWidth={1.5} />
      </div>
      <p className="text-sm font-semibold text-foreground">Nada por aqui</p>
      <p className="text-sm text-muted-foreground mt-1">{message}</p>
    </div>
  );
}
