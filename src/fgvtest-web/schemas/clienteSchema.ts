import { z } from 'zod';

export const criarClienteSchema = z.object({
  cnpj: z.string().min(14, 'CNPJ inválido').max(18, 'CNPJ inválido'),
  nome: z.string().min(1, 'Nome é obrigatório'),
  email: z.string().email('E-mail inválido'),
});

export type CriarClienteFormData = z.infer<typeof criarClienteSchema>;
