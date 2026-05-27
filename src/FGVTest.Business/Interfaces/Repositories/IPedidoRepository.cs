using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;

namespace FGVTest.Business.Interfaces.Repositories;

/// <summary>
/// Contrato do repositório de pedidos.
/// </summary>
public interface IPedidoRepository
{
    /// <summary>
    /// Obtém um pedido pelo identificador.
    /// </summary>
    Task<PedidoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken);

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
    /// Cria um novo pedido e retorna o identificador gerado.
    /// </summary>
    Task<int> CriarAsync(Pedido pedido, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza o valor total de um pedido.
    /// </summary>
    Task AtualizarValorTotalAsync(int codPedido, decimal valorTotal, CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona um item ao pedido.
    /// </summary>
    Task AdicionarItemAsync(ItensPedido item, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um item do pedido.
    /// </summary>
    Task RemoverItemAsync(int codPedido, int codProduto, CancellationToken cancellationToken);
}
