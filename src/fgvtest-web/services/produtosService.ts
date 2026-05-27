import api from './api';
import { Produto, CriarProdutoRequest } from '@/types/produto';
import { PagedResponse, PaginacaoParams } from '@/types/pagination';

export async function listarProdutos(params: PaginacaoParams): Promise<PagedResponse<Produto>> {
  const { data } = await api.get('/api/Produtos', {
    params: { Pagina: params.pagina, ItensPorPagina: params.itensPorPagina },
  });
  return data;
}

export async function obterProduto(id: number): Promise<Produto> {
  const { data } = await api.get(`/api/Produtos/${id}`);
  return data;
}

export async function criarProduto(data: CriarProdutoRequest): Promise<Produto> {
  const { data: res } = await api.post('/api/Produtos', data);
  return res;
}
