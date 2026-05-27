using AutoMapper;
using FGVTest.Api.ViewModels.Request;
using FGVTest.Api.ViewModels.Response;
using FGVTest.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FGVTest.Api.Controllers;

/// <summary>
/// Gerencia operações de clientes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ClientesController"/>.
    /// </summary>
    public ClientesController(IClienteService clienteService, IMapper mapper)
    {
        _clienteService = clienteService;
        _mapper = mapper;
    }

    /// <summary>
    /// Lista clientes com paginação.
    /// </summary>
    /// <param name="paginacao">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Lista paginada de clientes.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] PaginacaoRequest paginacao, CancellationToken cancellationToken)
    {
        var result = await _clienteService.ListarAsync(paginacao.Pagina, paginacao.ItensPorPagina, cancellationToken);
        var response = new PagedResponse<ClienteResponse>
        {
            Items = _mapper.Map<IEnumerable<ClienteResponse>>(result.Items),
            PaginaAtual = result.PaginaAtual,
            TotalPaginas = result.TotalPaginas,
            TotalItens = result.TotalItens,
            ItensPorPagina = result.ItensPorPagina
        };
        return Ok(response);
    }

    /// <summary>
    /// Obtém um cliente pelo CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do cliente encontrado.</returns>
    [HttpGet("cnpj/{cnpj}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorCnpj(string cnpj, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.ObterPorCnpjAsync(cnpj, cancellationToken);
        return Ok(_mapper.Map<ClienteResponse>(cliente));
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="request">Dados do cliente a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do cliente criado.</returns>
    /// <exception cref="ArgumentException">Lançada quando já existe um cliente com o CNPJ informado.</exception>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarClienteRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.CriarAsync(request.CNPJ, request.Nome, request.Email, cancellationToken);
        var response = _mapper.Map<ClienteResponse>(cliente);
        return CreatedAtAction(nameof(ObterPorCnpj), new { cnpj = response.CNPJ }, response);
    }
}
