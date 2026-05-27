import { z } from 'zod';

export const criarPedidoSchema = z.object({
  cnpj: z.string().min(14, 'CNPJ inválido'),
});

export const adicionarItemSchema = z.object({
  codProduto: z.number({ message: 'Selecione um produto' }),
  quantidade: z.preprocess(
    (val) => (val === '' ? undefined : Number(val)),
    z.number().int().positive('Quantidade deve ser maior que zero')
  ),
});

export type CriarPedidoFormData = z.infer<typeof criarPedidoSchema>;
export type AdicionarItemFormData = z.infer<typeof adicionarItemSchema>;
