namespace FGVTest.Business.DTOs;

public class ClienteDto
{
    public int CodCliente { get; set; }
    public string CNPJ { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
