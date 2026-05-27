using AutoMapper;
using FGVTest.Api.ViewModels.Request;
using FGVTest.Api.ViewModels.Response;
using FGVTest.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FGVTest.Api.Controllers;

/// <summary>
/// Gerencia operações de pedidos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PedidosController"/>.
    /// </summary>
    public PedidosController(IPedidoService pedidoService, IMapper mapper)
    {
        _pedidoService = pedidoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Lista pedidos com paginação e filtros opcionais.
    /// </summary>
    /// <param name="paginacao">Parâmetros de paginação.</param>
    /// <param name="dataInicio">Data de início do filtro.</param>
    /// <param name="dataFim">Data de fim do filtro.</param>
    /// <param name="codCliente">Código do cliente para filtrar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Lista paginada de pedidos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PedidoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] PaginacaoRequest paginacao,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int? codCliente,
        CancellationToken cancellationToken)
    {
        var result = await _pedidoService.ListarAsync(
            paginacao.Pagina, paginacao.ItensPorPagina, dataInicio, dataFim, codCliente, cancellationToken);

        var response = new PagedResponse<PedidoResponse>
        {
            Items = _mapper.Map<IEnumerable<PedidoResponse>>(result.Items),
            PaginaAtual = result.PaginaAtual,
            TotalPaginas = result.TotalPaginas,
            TotalItens = result.TotalItens,
            ItensPorPagina = result.ItensPorPagina
        };
        return Ok(response);
    }

    /// <summary>
    /// Obtém um pedido pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do pedido encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.ObterPorIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<PedidoResponse>(pedido));
    }

    /// <summary>
    /// Cria um novo pedido para o cliente identificado pelo CNPJ.
    /// </summary>
    /// <param name="request">Dados para criação do pedido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do pedido criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.CriarAsync(request.CNPJ, cancellationToken);
        var response = _mapper.Map<PedidoResponse>(pedido);
        return CreatedAtAction(nameof(ObterPorId), new { id = response.CodPedido }, response);
    }

    /// <summary>
    /// Adiciona um item ao pedido.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <param name="request">Dados do item a adicionar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados atualizados do pedido.</returns>
    [HttpPost("{id:int}/itens")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarItem(int id, [FromBody] AdicionarItemRequest request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.AdicionarItemAsync(id, request.CodProduto, request.Quantidade, cancellationToken);
        return Ok(_mapper.Map<PedidoResponse>(pedido));
    }

    /// <summary>
    /// Remove um item do pedido.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <param name="codProduto">Código do produto a remover.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados atualizados do pedido.</returns>
    [HttpDelete("{id:int}/itens/{codProduto:int}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverItem(int id, int codProduto, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.RemoverItemAsync(id, codProduto, cancellationToken);
        return Ok(_mapper.Map<PedidoResponse>(pedido));
    }
}
