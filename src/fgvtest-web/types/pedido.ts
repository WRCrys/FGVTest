import { Cliente } from './cliente';
import { Produto } from './produto';

export interface ItemPedido {
  produto: Produto;
  quantidade: number;
  precoUnitario: number;
}

export interface Pedido {
  codPedido: number;
  dataPedido: string;
  valorTotal: number;
  cliente: Cliente;
  itens: ItemPedido[];
}

export interface CriarPedidoRequest {
  cnpj: string;
}

export interface AdicionarItemRequest {
  codProduto: number;
  quantidade: number;
}
