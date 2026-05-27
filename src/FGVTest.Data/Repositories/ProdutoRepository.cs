using System.Data;
using Dapper;
using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Interfaces.Repositories;

namespace FGVTest.Data.Repositories;

/// <summary>
/// Repositório de produtos com Dapper.
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly IDbConnection _connection;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ProdutoRepository"/>.
    /// </summary>
    public ProdutoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Obtém um produto pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do produto ou null se não encontrado.</returns>
    public async Task<ProdutoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT CodProduto, Nome, Preco, Estoque FROM Produto WHERE CodProduto = @Id";
        return await _connection.QueryFirstOrDefaultAsync<ProdutoDto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Lista produtos com paginação.
    /// </summary>
    /// <param name="pagina">Número da página.</param>
    /// <param name="itensPorPagina">Quantidade de itens por página.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Resultado paginado de produtos.</returns>
    public async Task<PagedResultDto<ProdutoDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT CodProduto, Nome, Preco, Estoque
            FROM Produto
            ORDER BY CodProduto
            OFFSET (@Pagina - 1) * @ItensPorPagina ROWS
            FETCH NEXT @ItensPorPagina ROWS ONLY;

            SELECT COUNT(*) FROM Produto;";

        using var multi = await _connection.QueryMultipleAsync(
            new CommandDefinition(sql, new { Pagina = pagina, ItensPorPagina = itensPorPagina }, cancellationToken: cancellationToken));

        var items = await multi.ReadAsync<ProdutoDto>();
        var total = await multi.ReadFirstAsync<int>();

        return new PagedResultDto<ProdutoDto>
        {
            Items = items,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling((double)total / itensPorPagina)
        };
    }

    /// <summary>
    /// Cria um novo produto e retorna o identificador gerado.
    /// </summary>
    /// <param name="produto">Dados do produto a criar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Identificador do produto criado.</returns>
    public async Task<int> CriarAsync(Produto produto, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO Produto (Nome, Preco, Estoque)
            VALUES (@Nome, @Preco, @Estoque);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await _connection.QueryFirstAsync<int>(
            new CommandDefinition(sql, new
            {
                produto.Nome,
                produto.Preco,
                produto.Estoque
            }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Atualiza o estoque de um produto.
    /// </summary>
    /// <param name="codProduto">Código do produto.</param>
    /// <param name="novoEstoque">Novo valor do estoque.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    public async Task AtualizarEstoqueAsync(int codProduto, int novoEstoque, CancellationToken cancellationToken)
    {
        const string sql = "UPDATE Produto SET Estoque = @Estoque WHERE CodProduto = @CodProduto";
        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { CodProduto = codProduto, Estoque = novoEstoque }, cancellationToken: cancellationToken));
    }
}
