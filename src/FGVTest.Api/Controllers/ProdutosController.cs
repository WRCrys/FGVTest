using AutoMapper;
using FGVTest.Api.ViewModels.Request;
using FGVTest.Api.ViewModels.Response;
using FGVTest.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FGVTest.Api.Controllers;

/// <summary>
/// Gerencia operações de produtos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ProdutosController"/>.
    /// </summary>
    public ProdutosController(IProdutoService produtoService, IMapper mapper)
    {
        _produtoService = produtoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Lista produtos com paginação.
    /// </summary>
    /// <param name="paginacao">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Lista paginada de produtos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] PaginacaoRequest paginacao, CancellationToken cancellationToken)
    {
        var result = await _produtoService.ListarAsync(paginacao.Pagina, paginacao.ItensPorPagina, cancellationToken);
        var response = new PagedResponse<ProdutoResponse>
        {
            Items = _mapper.Map<IEnumerable<ProdutoResponse>>(result.Items),
            PaginaAtual = result.PaginaAtual,
            TotalPaginas = result.TotalPaginas,
            TotalItens = result.TotalItens,
            ItensPorPagina = result.ItensPorPagina
        };
        return Ok(response);
    }

    /// <summary>
    /// Obtém um produto pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do produto encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.ObterPorIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<ProdutoResponse>(produto));
    }

    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    /// <param name="request">Dados do produto a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do produto criado.</returns>
    /// <exception cref="ArgumentException">Lançada quando os dados do produto são inválidos.</exception>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoRequest request, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.CriarAsync(request.Nome, request.Preco, request.Estoque, cancellationToken);
        var response = _mapper.Map<ProdutoResponse>(produto);
        return CreatedAtAction(nameof(ObterPorId), new { id = response.CodProduto }, response);
    }
}
