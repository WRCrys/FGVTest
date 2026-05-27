namespace FGVTest.Api.ViewModels.Request;

/// <summary>
/// Dados para criação de um cliente.
/// </summary>
public class CriarClienteRequest
{
    /// <summary>CNPJ do cliente.</summary>
    public string CNPJ { get; set; } = string.Empty;

    /// <summary>Nome do cliente.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail do cliente.</summary>
    public string Email { get; set; } = string.Empty;
}
