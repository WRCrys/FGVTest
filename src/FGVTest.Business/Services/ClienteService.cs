using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Exceptions;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Interfaces.Services;

namespace FGVTest.Business.Services;

/// <summary>
/// Serviço responsável pelas regras de negócio de clientes.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ClienteService"/>.
    /// </summary>
    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    /// <summary>
    /// Obtém um cliente pelo CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do cliente encontrado.</returns>
    /// <exception cref="NotFoundException">Lançada quando o cliente não é encontrado.</exception>
    public async Task<ClienteDto> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterPorCnpjAsync(cnpj, cancellationToken);
        if (cliente is null)
            throw new NotFoundException("Cliente não encontrado.");
        return cliente;
    }

    /// <summary>
    /// Lista clientes com paginação.
    /// </summary>
    /// <param name="pagina">Número da página.</param>
    /// <param name="itensPorPagina">Quantidade de itens por página.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Resultado paginado de clientes.</returns>
    public async Task<PagedResultDto<ClienteDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken)
    {
        return await _clienteRepository.ListarAsync(pagina, itensPorPagina, cancellationToken);
    }

    /// <summary>
    /// Cria um novo cliente no sistema.
    /// </summary>
    /// <param name="cnpj">CNPJ do cliente.</param>
    /// <param name="nome">Nome do cliente.</param>
    /// <param name="email">E-mail do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do cliente criado.</returns>
    /// <exception cref="ArgumentException">Lançada quando já existe um cliente com o CNPJ informado.</exception>
    public async Task<ClienteDto> CriarAsync(string cnpj, string nome, string email, CancellationToken cancellationToken)
    {
        var existente = await _clienteRepository.ObterPorCnpjAsync(cnpj, cancellationToken);
        if (existente is not null)
            throw new ArgumentException("Já existe um cliente cadastrado com este CNPJ.");

        var cliente = new Cliente
        {
            CNPJ = cnpj,
            Nome = nome,
            Email = email,
            DataCadastro = DateTime.UtcNow
        };

        var id = await _clienteRepository.CriarAsync(cliente, cancellationToken);
        return await _clienteRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException("Cliente não encontrado após criação.");
    }
}
