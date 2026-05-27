using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;

namespace FGVTest.Business.Interfaces.Repositories;

/// <summary>
/// Contrato do repositório de produtos.
/// </summary>
public interface IProdutoRepository
{
    /// <summary>
    /// Obtém um produto pelo identificador.
    /// </summary>
    Task<ProdutoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Lista produtos com paginação.
    /// </summary>
    Task<PagedResultDto<ProdutoDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo produto e retorna o identificador gerado.
    /// </summary>
    Task<int> CriarAsync(Produto produto, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza o estoque de um produto.
    /// </summary>
    Task AtualizarEstoqueAsync(int codProduto, int novoEstoque, CancellationToken cancellationToken);
}
