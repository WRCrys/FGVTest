namespace FGVTest.Business.DTOs;

public class ItensPedidoDto
{
    public int CodPedido { get; set; }
    public int CodProduto { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public ProdutoDto Produto { get; set; } = null!;
}
