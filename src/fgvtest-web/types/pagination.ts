export interface PagedResponse<T> {
  items: T[];
  paginaAtual: number;
  totalPaginas: number;
  totalItens: number;
  itensPorPagina: number;
}

export interface PaginacaoParams {
  pagina?: number;
  itensPorPagina?: number;
}
