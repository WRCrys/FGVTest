namespace FGVTest.Api.ViewModels.Request;

/// <summary>
/// Dados para criação de um pedido.
/// </summary>
public class CriarPedidoRequest
{
    /// <summary>CNPJ do cliente para o qual o pedido será criado.</summary>
    public string CNPJ { get; set; } = string.Empty;
}
