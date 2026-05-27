namespace FGVTest.Api.ViewModels.Response;

/// <summary>
/// Dados públicos de um cliente.
/// </summary>
public class ClienteResponse
{
    /// <summary>Identificador do cliente.</summary>
    public int CodCliente { get; set; }

    /// <summary>CNPJ do cliente.</summary>
    public string CNPJ { get; set; } = string.Empty;

    /// <summary>Nome do cliente.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail do cliente.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Data de cadastro do cliente.</summary>
    public DateTime DataCadastro { get; set; }
}
