using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FGVTest.Test.E2E.Infrastructure;

public class TestProdutoRepository : IProdutoRepository
{
    private readonly TestDbContext _db;

    public TestProdutoRepository(TestDbContext db)
    {
        _db = db;
    }

    public async Task<ProdutoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var e = await _db.Produtos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodProduto == id, cancellationToken);
        return e is null ? null : MapDto(e);
    }

    public async Task<PagedResultDto<ProdutoDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken)
    {
        var total = await _db.Produtos.CountAsync(cancellationToken);
        var items = await _db.Produtos.AsNoTracking()
            .OrderBy(p => p.CodProduto)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ProdutoDto>
        {
            Items = items.Select(MapDto),
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling((double)total / itensPorPagina)
        };
    }

    public async Task<int> CriarAsync(Produto produto, CancellationToken cancellationToken)
    {
        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync(cancellationToken);
        return produto.CodProduto;
    }

    public async Task AtualizarEstoqueAsync(int codProduto, int novoEstoque, CancellationToken cancellationToken)
    {
        var produto = await _db.Produtos.FindAsync([codProduto], cancellationToken);
        if (produto is not null)
        {
            produto.Estoque = novoEstoque;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private static ProdutoDto MapDto(Produto e) => new()
    {
        CodProduto = e.CodProduto,
        Nome = e.Nome,
        Preco = e.Preco,
        Estoque = e.Estoque
    };
}
