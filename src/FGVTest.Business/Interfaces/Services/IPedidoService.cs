using FGVTest.Business.DTOs;

namespace FGVTest.Business.Interfaces.Services;

/// <summary>
/// Contrato do serviço de pedidos.
/// </summary>
public interface IPedidoService
{
    /// <summary>
    /// Obtém um pedido pelo identificador.
    /// </summary>
    Task<PedidoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Lista pedidos com paginação e filtros opcionais.
    /// </summary>
    Task<PagedResultDto<PedidoDto>> ListarAsync(
        int pagina,
        int itensPorPagina,
        DateTime? dataInicio,
        DateTime? dataFim,
        int? codCliente,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo pedido para o cliente identificado pelo CNPJ.
    /// </summary>
    Task<PedidoDto> CriarAsync(string cnpj, CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona um item ao pedido.
    /// </summary>
    Task<PedidoDto> AdicionarItemAsync(int codPedido, int codProduto, int quantidade, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um item do pedido e devolve a quantidade ao estoque.
    /// </summary>
    Task<PedidoDto> RemoverItemAsync(int codPedido, int codProduto, CancellationToken cancellationToken);
}
