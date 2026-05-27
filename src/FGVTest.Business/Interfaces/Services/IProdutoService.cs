using FGVTest.Business.DTOs;

namespace FGVTest.Business.Interfaces.Services;

/// <summary>
/// Contrato do serviço de produtos.
/// </summary>
public interface IProdutoService
{
    /// <summary>
    /// Obtém um produto pelo identificador.
    /// </summary>
    Task<ProdutoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Lista produtos com paginação.
    /// </summary>
    Task<PagedResultDto<ProdutoDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    Task<ProdutoDto> CriarAsync(string nome, decimal preco, int estoque, CancellationToken cancellationToken);
}
