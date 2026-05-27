import { AlertCircle } from 'lucide-react';

interface ErrorMessageProps {
  message?: string;
}

export function ErrorMessage({ message = 'Ocorreu um erro ao carregar os dados.' }: ErrorMessageProps) {
  return (
    <div className="flex items-start gap-3 p-4 rounded-xl bg-destructive/5 border border-destructive/20">
      <AlertCircle size={18} className="text-destructive shrink-0 mt-0.5" strokeWidth={1.75} />
      <p className="text-sm text-destructive">{message}</p>
    </div>
  );
}
