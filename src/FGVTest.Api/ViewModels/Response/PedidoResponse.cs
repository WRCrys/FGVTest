namespace FGVTest.Api.ViewModels.Response;

/// <summary>
/// Dados completos de um pedido.
/// </summary>
public class PedidoResponse
{
    /// <summary>Identificador do pedido.</summary>
    public int CodPedido { get; set; }

    /// <summary>Data de criação do pedido.</summary>
    public DateTime DataPedido { get; set; }

    /// <summary>Valor total do pedido.</summary>
    public decimal ValorTotal { get; set; }

    /// <summary>Dados do cliente.</summary>
    public ClienteResponse Cliente { get; set; } = null!;

    /// <summary>Itens do pedido.</summary>
    public List<ItemPedidoResponse> Itens { get; set; } = [];
}
