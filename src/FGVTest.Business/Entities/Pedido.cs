namespace FGVTest.Business.Entities;

public class Pedido
{
    public int CodPedido { get; set; }
    public int CodCliente { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal ValorTotal { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public IEnumerable<ItensPedido> Itens { get; set; } = [];
}
