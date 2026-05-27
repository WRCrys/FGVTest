using System.Data;
using Dapper;
using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Interfaces.Repositories;

namespace FGVTest.Data.Repositories;

/// <summary>
/// Repositório de pedidos com Dapper.
/// </summary>
public class PedidoRepository : IPedidoRepository
{
    private readonly IDbConnection _connection;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PedidoRepository"/>.
    /// </summary>
    public PedidoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Obtém um pedido completo (com cliente e itens) pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do pedido ou null se não encontrado.</returns>
    public async Task<PedidoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        const string pedidoSql = @"
            SELECT p.CodPedido, p.CodCliente, p.DataPedido, p.ValorTotal
            FROM Pedido p
            WHERE p.CodPedido = @Id";

        var pedido = await _connection.QueryFirstOrDefaultAsync<PedidoDto>(
            new CommandDefinition(pedidoSql, new { Id = id }, cancellationToken: cancellationToken));

        if (pedido is null)
            return null;

        const string clienteSql = @"
            SELECT CodCliente, CNPJ, Nome, Email, DataCadastro
            FROM Cliente WHERE CodCliente = @CodCliente";

        var cliente = await _connection.QueryFirstOrDefaultAsync<ClienteDto>(
            new CommandDefinition(clienteSql, new { CodCliente = pedido.CodCliente }, cancellationToken: cancellationToken));

        const string itensSql = @"
            SELECT ip.CodPedido, ip.CodProduto, ip.Quantidade, ip.PrecoUnitario,
                   pr.CodProduto, pr.Nome, pr.Preco, pr.Estoque
            FROM ItensPedido ip
            INNER JOIN Produto pr ON pr.CodProduto = ip.CodProduto
            WHERE ip.CodPedido = @CodPedido";

        var itens = await _connection.QueryAsync<ItensPedidoDto, ProdutoDto, ItensPedidoDto>(
            new CommandDefinition(itensSql, new { CodPedido = id }, cancellationToken: cancellationToken),
            (item, produto) => { item.Produto = produto; return item; },
            splitOn: "CodProduto");

        pedido.Cliente = cliente ?? new ClienteDto();
        pedido.Itens = itens;
        return pedido;
    }

    /// <summary>
    /// Lista pedidos com paginação e filtros opcionais.
    /// </summary>
    /// <param name="pagina">Número da página.</param>
    /// <param name="itensPorPagina">Quantidade de itens por página.</param>
    /// <param name="dataInicio">Data de início do filtro.</param>
    /// <param name="dataFim">Data de fim do filtro.</param>
    /// <param name="codCliente">Código do cliente para filtrar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Resultado paginado de pedidos.</returns>
    public async Task<PagedResultDto<PedidoDto>> ListarAsync(
        int pagina,
        int itensPorPagina,
        DateTime? dataInicio,
        DateTime? dataFim,
        int? codCliente,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Pagina", pagina);
        parameters.Add("@ItensPorPagina", itensPorPagina);

        var where = "WHERE 1=1";

        if (dataInicio.HasValue)
        {
            where += " AND p.DataPedido >= @DataInicio";
            parameters.Add("@DataInicio", dataInicio.Value);
        }

        if (dataFim.HasValue)
        {
            where += " AND p.DataPedido <= @DataFim";
            parameters.Add("@DataFim", dataFim.Value);
        }

        if (codCliente.HasValue)
        {
            where += " AND p.CodCliente = @CodCliente";
            parameters.Add("@CodCliente", codCliente.Value);
        }

        var sql = $@"
            SELECT p.CodPedido, p.CodCliente, p.DataPedido, p.ValorTotal
            FROM Pedido p
            {where}
            ORDER BY p.CodPedido
            OFFSET (@Pagina - 1) * @ItensPorPagina ROWS
            FETCH NEXT @ItensPorPagina ROWS ONLY;

            SELECT COUNT(*) FROM Pedido p {where};";

        using var multi = await _connection.QueryMultipleAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var pedidosRaw = (await multi.ReadAsync<PedidoDto>()).ToList();
        var total = await multi.ReadFirstAsync<int>();

        if (pedidosRaw.Count > 0)
        {
            var ids = pedidosRaw.Select(p => p.CodPedido).ToArray();
            var codClienteIds = pedidosRaw.Select(p => p.CodCliente).Distinct().ToArray();

            const string clientesSql = "SELECT CodCliente, CNPJ, Nome, Email, DataCadastro FROM Cliente WHERE CodCliente IN @Ids";
            var clientes = (await _connection.QueryAsync<ClienteDto>(
                new CommandDefinition(clientesSql, new { Ids = codClienteIds }, cancellationToken: cancellationToken)))
                .ToDictionary(c => c.CodCliente);

            const string itensSql = @"
                SELECT ip.CodPedido, ip.CodProduto, ip.Quantidade, ip.PrecoUnitario,
                       pr.CodProduto, pr.Nome, pr.Preco, pr.Estoque
                FROM ItensPedido ip
                INNER JOIN Produto pr ON pr.CodProduto = ip.CodProduto
                WHERE ip.CodPedido IN @Ids";

            var todosItens = (await _connection.QueryAsync<ItensPedidoDto, ProdutoDto, ItensPedidoDto>(
                new CommandDefinition(itensSql, new { Ids = ids }, cancellationToken: cancellationToken),
                (item, produto) => { item.Produto = produto; return item; },
                splitOn: "CodProduto")).ToLookup(i => i.CodPedido);

            foreach (var pedido in pedidosRaw)
            {
                pedido.Cliente = clientes.TryGetValue(pedido.CodCliente, out var c) ? c : new ClienteDto();
                pedido.Itens = todosItens[pedido.CodPedido];
            }
        }

        return new PagedResultDto<PedidoDto>
        {
            Items = pedidosRaw,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling((double)total / itensPorPagina)
        };
    }

    /// <summary>
    /// Cria um novo pedido e retorna o identificador gerado.
    /// </summary>
    /// <param name="pedido">Dados do pedido a criar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Identificador do pedido criado.</returns>
    public async Task<int> CriarAsync(Pedido pedido, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO Pedido (CodCliente, DataPedido, ValorTotal)
            VALUES (@CodCliente, @DataPedido, @ValorTotal);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await _connection.QueryFirstAsync<int>(
            new CommandDefinition(sql, new
            {
                pedido.CodCliente,
                pedido.DataPedido,
                pedido.ValorTotal
            }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Atualiza o valor total de um pedido.
    /// </summary>
    /// <param name="codPedido">Código do pedido.</param>
    /// <param name="valorTotal">Novo valor total.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    public async Task AtualizarValorTotalAsync(int codPedido, decimal valorTotal, CancellationToken cancellationToken)
    {
        const string sql = "UPDATE Pedido SET ValorTotal = @ValorTotal WHERE CodPedido = @CodPedido";
        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { CodPedido = codPedido, ValorTotal = valorTotal }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Adiciona um item ao pedido.
    /// </summary>
    /// <param name="item">Dados do item a adicionar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    public async Task AdicionarItemAsync(ItensPedido item, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO ItensPedido (CodPedido, CodProduto, Quantidade, PrecoUnitario)
            VALUES (@CodPedido, @CodProduto, @Quantidade, @PrecoUnitario)";

        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                item.CodPedido,
                item.CodProduto,
                item.Quantidade,
                item.PrecoUnitario
            }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Remove um item do pedido.
    /// </summary>
    /// <param name="codPedido">Código do pedido.</param>
    /// <param name="codProduto">Código do produto a remover.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    public async Task RemoverItemAsync(int codPedido, int codProduto, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM ItensPedido WHERE CodPedido = @CodPedido AND CodProduto = @CodProduto";
        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { CodPedido = codPedido, CodProduto = codProduto }, cancellationToken: cancellationToken));
    }
}
