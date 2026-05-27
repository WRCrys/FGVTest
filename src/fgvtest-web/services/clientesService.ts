import api from './api';
import { Cliente, CriarClienteRequest } from '@/types/cliente';
import { PagedResponse, PaginacaoParams } from '@/types/pagination';

export async function listarClientes(params: PaginacaoParams): Promise<PagedResponse<Cliente>> {
  const { data } = await api.get('/api/Clientes', {
    params: { Pagina: params.pagina, ItensPorPagina: params.itensPorPagina },
  });
  return data;
}

export async function obterClientePorCnpj(cnpj: string): Promise<Cliente> {
  const { data } = await api.get(`/api/Clientes/cnpj/${cnpj}`);
  return data;
}

export async function criarCliente(data: CriarClienteRequest): Promise<Cliente> {
  const { data: res } = await api.post('/api/Clientes', data);
  return res;
}
