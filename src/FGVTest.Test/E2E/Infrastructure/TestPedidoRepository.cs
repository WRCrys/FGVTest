using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FGVTest.Test.E2E.Infrastructure;

public class TestPedidoRepository : IPedidoRepository
{
    private readonly TestDbContext _db;

    public TestPedidoRepository(TestDbContext db)
    {
        _db = db;
    }

    public async Task<PedidoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var pedido = await _db.Pedidos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodPedido == id, cancellationToken);
        if (pedido is null) return null;

        var cliente = await _db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CodCliente == pedido.CodCliente, cancellationToken);

        var itensRaw = await _db.ItensPedido.AsNoTracking()
            .Where(i => i.CodPedido == id)
            .ToListAsync(cancellationToken);

        var produtosIds = itensRaw.Select(i => i.CodProduto).ToArray();
        var produtos = await _db.Produtos.AsNoTracking()
            .Where(p => produtosIds.Contains(p.CodProduto))
            .ToDictionaryAsync(p => p.CodProduto, cancellationToken);

        return BuildPedidoDto(pedido, cliente, itensRaw, produtos);
    }

    public async Task<PagedResultDto<PedidoDto>> ListarAsync(
        int pagina,
        int itensPorPagina,
        DateTime? dataInicio,
        DateTime? dataFim,
        int? codCliente,
        CancellationToken cancellationToken)
    {
        var query = _db.Pedidos.AsNoTracking().AsQueryable();

        if (dataInicio.HasValue) query = query.Where(p => p.DataPedido >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(p => p.DataPedido <= dataFim.Value);
        if (codCliente.HasValue) query = query.Where(p => p.CodCliente == codCliente.Value);

        var total = await query.CountAsync(cancellationToken);
        var pedidos = await query.OrderBy(p => p.CodPedido)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync(cancellationToken);

        var clienteIds = pedidos.Select(p => p.CodCliente).Distinct().ToArray();
        var clientes = await _db.Clientes.AsNoTracking()
            .Where(c => clienteIds.Contains(c.CodCliente))
            .ToDictionaryAsync(c => c.CodCliente, cancellationToken);

        var pedidoIds = pedidos.Select(p => p.CodPedido).ToArray();
        var todosItens = await _db.ItensPedido.AsNoTracking()
            .Where(i => pedidoIds.Contains(i.CodPedido))
            .ToListAsync(cancellationToken);

        var produtoIds = todosItens.Select(i => i.CodProduto).Distinct().ToArray();
        var produtos = await _db.Produtos.AsNoTracking()
            .Where(p => produtoIds.Contains(p.CodProduto))
            .ToDictionaryAsync(p => p.CodProduto, cancellationToken);

        var dtos = pedidos.Select(p =>
        {
            var itensRaw = todosItens.Where(i => i.CodPedido == p.CodPedido).ToList();
            clientes.TryGetValue(p.CodCliente, out var c);
            return BuildPedidoDto(p, c, itensRaw, produtos);
        }).ToList();

        return new PagedResultDto<PedidoDto>
        {
            Items = dtos,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling((double)total / itensPorPagina)
        };
    }

    public async Task<int> CriarAsync(Pedido pedido, CancellationToken cancellationToken)
    {
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync(cancellationToken);
        return pedido.CodPedido;
    }

    public async Task AtualizarValorTotalAsync(int codPedido, decimal valorTotal, CancellationToken cancellationToken)
    {
        var pedido = await _db.Pedidos.FindAsync([codPedido], cancellationToken);
        if (pedido is not null)
        {
            pedido.ValorTotal = valorTotal;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AdicionarItemAsync(ItensPedido item, CancellationToken cancellationToken)
    {
        _db.ItensPedido.Add(item);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverItemAsync(int codPedido, int codProduto, CancellationToken cancellationToken)
    {
        var item = await _db.ItensPedido
            .FirstOrDefaultAsync(i => i.CodPedido == codPedido && i.CodProduto == codProduto, cancellationToken);
        if (item is not null)
        {
            _db.ItensPedido.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private static PedidoDto BuildPedidoDto(
        Pedido pedido,
        Cliente? cliente,
        List<ItensPedido> itensRaw,
        Dictionary<int, Produto> produtos)
    {
        var itens = itensRaw.Select(i => new ItensPedidoDto
        {
            CodPedido = i.CodPedido,
            CodProduto = i.CodProduto,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.PrecoUnitario,
            Produto = produtos.TryGetValue(i.CodProduto, out var p)
                ? new ProdutoDto { CodProduto = p.CodProduto, Nome = p.Nome, Preco = p.Preco, Estoque = p.Estoque }
                : new ProdutoDto()
        });

        return new PedidoDto
        {
            CodPedido = pedido.CodPedido,
            CodCliente = pedido.CodCliente,
            DataPedido = pedido.DataPedido,
            ValorTotal = pedido.ValorTotal,
            Cliente = cliente is null ? new ClienteDto() : new ClienteDto
            {
                CodCliente = cliente.CodCliente,
                CNPJ = cliente.CNPJ,
                Nome = cliente.Nome,
                Email = cliente.Email,
                DataCadastro = cliente.DataCadastro
            },
            Itens = itens
        };
    }
}
