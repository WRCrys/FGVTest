'use client';

import { useQuery } from '@tanstack/react-query';
import { listarPedidos } from '@/services/pedidosService';

interface UsePedidosParams {
  pagina: number;
  itensPorPagina: number;
  dataInicio?: string;
  dataFim?: string;
  codCliente?: number;
}

export function usePedidos(params: UsePedidosParams) {
  return useQuery({
    queryKey: ['pedidos', params],
    queryFn: () => listarPedidos(params),
  });
}
