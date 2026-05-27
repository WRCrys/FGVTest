import api from './api';
import { Pedido, CriarPedidoRequest, AdicionarItemRequest } from '@/types/pedido';
import { PagedResponse } from '@/types/pagination';

interface ListarPedidosParams {
  pagina: number;
  itensPorPagina: number;
  dataInicio?: string;
  dataFim?: string;
  codCliente?: number;
}

export async function listarPedidos(params: ListarPedidosParams): Promise<PagedResponse<Pedido>> {
  const { data } = await api.get('/api/Pedidos', {
    params: {
      Pagina: params.pagina,
      ItensPorPagina: params.itensPorPagina,
      dataInicio: params.dataInicio,
      dataFim: params.dataFim,
      codCliente: params.codCliente,
    },
  });
  return data;
}

export async function obterPedido(id: number): Promise<Pedido> {
  const { data } = await api.get(`/api/Pedidos/${id}`);
  return data;
}

export async function criarPedido(data: CriarPedidoRequest): Promise<Pedido> {
  const { data: res } = await api.post('/api/Pedidos', data);
  return res;
}

export async function adicionarItem(pedidoId: number, data: AdicionarItemRequest): Promise<Pedido> {
  const { data: res } = await api.post(`/api/Pedidos/${pedidoId}/itens`, data);
  return res;
}

export async function removerItem(pedidoId: number, codProduto: number): Promise<Pedido> {
  const { data } = await api.delete(`/api/Pedidos/${pedidoId}/itens/${codProduto}`);
  return data;
}
