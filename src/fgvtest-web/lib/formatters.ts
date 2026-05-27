export function formatarCNPJ(cnpj: string): string {
  const digits = cnpj.replace(/\D/g, '');
  return digits.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
}

export function formatarMoeda(valor: number): string {
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
}

export function formatarData(data: string): string {
  return new Intl.DateTimeFormat('pt-BR').format(new Date(data));
}

export function limparCNPJ(cnpj: string): string {
  return cnpj.replace(/\D/g, '');
}
