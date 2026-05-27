export interface Cliente {
  codCliente: number;
  cnpj: string;
  nome: string;
  email: string;
  dataCadastro: string;
}

export interface CriarClienteRequest {
  cnpj: string;
  nome: string;
  email: string;
}
