'use client';

import { useQuery } from '@tanstack/react-query';
import { listarProdutos } from '@/services/produtosService';
import { PaginacaoParams } from '@/types/pagination';

export function useProdutos(params: PaginacaoParams) {
  return useQuery({
    queryKey: ['produtos', params],
    queryFn: () => listarProdutos(params),
  });
}
