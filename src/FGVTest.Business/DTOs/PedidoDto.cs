namespace FGVTest.Business.DTOs;

public class PedidoDto
{
    public int CodPedido { get; set; }
    public int CodCliente { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal ValorTotal { get; set; }
    public ClienteDto Cliente { get; set; } = null!;
    public IEnumerable<ItensPedidoDto> Itens { get; set; } = [];
}
