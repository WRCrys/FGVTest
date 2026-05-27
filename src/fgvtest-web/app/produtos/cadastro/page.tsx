import { PageHeader } from '@/components/common/PageHeader';
import { ProdutoForm } from '@/components/produtos/ProdutoForm';

export default function CadastroProdutoPage() {
  return (
    <div>
      <PageHeader title="Cadastrar Produto" />
      <ProdutoForm />
    </div>
  );
}
