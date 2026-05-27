import { z } from 'zod';

export const criarProdutoSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  preco: z.number({ message: 'Preço deve ser um número' }).positive('Preço deve ser maior que zero'),
  estoque: z.number({ message: 'Estoque deve ser um número' }).int().min(0, 'Estoque não pode ser negativo'),
});

export type CriarProdutoFormData = z.infer<typeof criarProdutoSchema>;
