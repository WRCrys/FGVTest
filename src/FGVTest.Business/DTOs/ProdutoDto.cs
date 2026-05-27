namespace FGVTest.Business.DTOs;

public class ProdutoDto
{
    public int CodProduto { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
}
