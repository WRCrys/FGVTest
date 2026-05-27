import { PageHeader } from '@/components/common/PageHeader';
import { ClienteForm } from '@/components/clientes/ClienteForm';

export default function CadastroClientePage() {
  return (
    <div>
      <PageHeader title="Cadastrar Cliente" />
      <ClienteForm />
    </div>
  );
}
