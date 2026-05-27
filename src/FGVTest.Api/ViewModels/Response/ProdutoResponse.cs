namespace FGVTest.Api.ViewModels.Response;

/// <summary>
/// Dados públicos de um produto.
/// </summary>
public class ProdutoResponse
{
    /// <summary>Identificador do produto.</summary>
    public int CodProduto { get; set; }

    /// <summary>Nome do produto.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Preço do produto.</summary>
    public decimal Preco { get; set; }

    /// <summary>Estoque disponível.</summary>
    public int Estoque { get; set; }
}
