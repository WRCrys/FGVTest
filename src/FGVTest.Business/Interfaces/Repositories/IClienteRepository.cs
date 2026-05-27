using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;

namespace FGVTest.Business.Interfaces.Repositories;

/// <summary>
/// Contrato do repositório de clientes.
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Obtém um cliente pelo CNPJ.
    /// </summary>
    Task<ClienteDto?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém um cliente pelo identificador.
    /// </summary>
    Task<ClienteDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Lista clientes com paginação.
    /// </summary>
    Task<PagedResultDto<ClienteDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo cliente e retorna o identificador gerado.
    /// </summary>
    Task<int> CriarAsync(Cliente cliente, CancellationToken cancellationToken);
}
