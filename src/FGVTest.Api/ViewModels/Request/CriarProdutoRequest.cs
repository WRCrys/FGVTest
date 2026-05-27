namespace FGVTest.Api.ViewModels.Request;

/// <summary>
/// Dados para criação de um produto.
/// </summary>
public class CriarProdutoRequest
{
    /// <summary>Nome do produto.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Preço do produto.</summary>
    public decimal Preco { get; set; }

    /// <summary>Estoque inicial do produto.</summary>
    public int Estoque { get; set; }
}
