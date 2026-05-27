using FGVTest.Business.DTOs;

namespace FGVTest.Business.Interfaces.Services;

/// <summary>
/// Contrato do serviço de clientes.
/// </summary>
public interface IClienteService
{
    /// <summary>
    /// Obtém um cliente pelo CNPJ.
    /// </summary>
    Task<ClienteDto> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken);

    /// <summary>
    /// Lista clientes com paginação.
    /// </summary>
    Task<PagedResultDto<ClienteDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    Task<ClienteDto> CriarAsync(string cnpj, string nome, string email, CancellationToken cancellationToken);
}
