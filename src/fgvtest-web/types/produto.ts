export interface Produto {
  codProduto: number;
  nome: string;
  preco: number;
  estoque: number;
}

export interface CriarProdutoRequest {
  nome: string;
  preco: number;
  estoque: number;
}
