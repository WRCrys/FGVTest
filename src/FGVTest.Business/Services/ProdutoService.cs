using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Exceptions;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Interfaces.Services;

namespace FGVTest.Business.Services;

/// <summary>
/// Serviço responsável pelas regras de negócio de produtos.
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ProdutoService"/>.
    /// </summary>
    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    /// <summary>
    /// Obtém um produto pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do produto encontrado.</returns>
    /// <exception cref="NotFoundException">Lançada quando o produto não é encontrado.</exception>
    public async Task<ProdutoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id, cancellationToken);
        if (produto is null)
            throw new NotFoundException("Produto não encontrado.");
        return produto;
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
        return await _produtoRepository.ListarAsync(pagina, itensPorPagina, cancellationToken);
    }

    /// <summary>
    /// Cria um novo produto no sistema.
    /// </summary>
    /// <param name="nome">Nome do produto.</param>
    /// <param name="preco">Preço do produto.</param>
    /// <param name="estoque">Estoque inicial do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do produto criado.</returns>
    /// <exception cref="ArgumentException">Lançada quando os dados do produto são inválidos.</exception>
    public async Task<ProdutoDto> CriarAsync(string nome, decimal preco, int estoque, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do produto não pode ser vazio.");
        if (preco <= 0)
            throw new ArgumentException("O preço do produto deve ser maior que zero.");
        if (estoque < 0)
            throw new ArgumentException("O estoque do produto não pode ser negativo.");

        var produto = new Produto
        {
            Nome = nome,
            Preco = preco,
            Estoque = estoque
        };

        var id = await _produtoRepository.CriarAsync(produto, cancellationToken);
        return await _produtoRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException("Produto não encontrado após criação.");
    }
}
