namespace FGVTest.Api.ViewModels.Response;

/// <summary>
/// Dados de um item do pedido.
/// </summary>
public class ItemPedidoResponse
{
    /// <summary>Dados do produto.</summary>
    public ProdutoResponse Produto { get; set; } = null!;

    /// <summary>Quantidade do item.</summary>
    public int Quantidade { get; set; }

    /// <summary>Preço unitário no momento da adição.</summary>
    public decimal PrecoUnitario { get; set; }
}
