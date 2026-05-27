'use client';

import { useQuery } from '@tanstack/react-query';
import { listarClientes } from '@/services/clientesService';
import { PaginacaoParams } from '@/types/pagination';

export function useClientes(params: PaginacaoParams) {
  return useQuery({
    queryKey: ['clientes', params],
    queryFn: () => listarClientes(params),
  });
}
