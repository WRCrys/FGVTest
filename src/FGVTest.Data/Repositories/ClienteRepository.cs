using System.Data;
using Dapper;
using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Interfaces.Repositories;

namespace FGVTest.Data.Repositories;

/// <summary>
/// Repositório de clientes com Dapper.
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly IDbConnection _connection;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ClienteRepository"/>.
    /// </summary>
    public ClienteRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Obtém um cliente pelo CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do cliente ou null se não encontrado.</returns>
    public async Task<ClienteDto?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        const string sql = "SELECT CodCliente, CNPJ, Nome, Email, DataCadastro FROM Cliente WHERE CNPJ = @Cnpj";
        return await _connection.QueryFirstOrDefaultAsync<ClienteDto>(
            new CommandDefinition(sql, new { Cnpj = cnpj }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Obtém um cliente pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do cliente ou null se não encontrado.</returns>
    public async Task<ClienteDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT CodCliente, CNPJ, Nome, Email, DataCadastro FROM Cliente WHERE CodCliente = @Id";
        return await _connection.QueryFirstOrDefaultAsync<ClienteDto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
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
        const string sql = @"
            SELECT CodCliente, CNPJ, Nome, Email, DataCadastro
            FROM Cliente
            ORDER BY CodCliente
            OFFSET (@Pagina - 1) * @ItensPorPagina ROWS
            FETCH NEXT @ItensPorPagina ROWS ONLY;

            SELECT COUNT(*) FROM Cliente;";

        using var multi = await _connection.QueryMultipleAsync(
            new CommandDefinition(sql, new { Pagina = pagina, ItensPorPagina = itensPorPagina }, cancellationToken: cancellationToken));

        var items = await multi.ReadAsync<ClienteDto>();
        var total = await multi.ReadFirstAsync<int>();

        return new PagedResultDto<ClienteDto>
        {
            Items = items,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling((double)total / itensPorPagina)
        };
    }

    /// <summary>
    /// Cria um novo cliente e retorna o identificador gerado.
    /// </summary>
    /// <param name="cliente">Dados do cliente a criar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Identificador do cliente criado.</returns>
    public async Task<int> CriarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO Cliente (CNPJ, Nome, Email, DataCadastro)
            VALUES (@CNPJ, @Nome, @Email, @DataCadastro);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await _connection.QueryFirstAsync<int>(
            new CommandDefinition(sql, new
            {
                cliente.CNPJ,
                cliente.Nome,
                cliente.Email,
                cliente.DataCadastro
            }, cancellationToken: cancellationToken));
    }
}
