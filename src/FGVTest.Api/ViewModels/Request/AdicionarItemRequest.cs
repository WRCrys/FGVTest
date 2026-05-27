namespace FGVTest.Api.ViewModels.Request;

/// <summary>
/// Dados para adição de um item ao pedido.
/// </summary>
public class AdicionarItemRequest
{
    /// <summary>Código do produto.</summary>
    public int CodProduto { get; set; }

    /// <summary>Quantidade a adicionar.</summary>
    public int Quantidade { get; set; }
}
