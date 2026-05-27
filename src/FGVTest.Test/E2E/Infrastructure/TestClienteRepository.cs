using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FGVTest.Test.E2E.Infrastructure;

public class TestClienteRepository : IClienteRepository
{
    private readonly TestDbContext _db;

    public TestClienteRepository(TestDbContext db)
    {
        _db = db;
    }

    public async Task<ClienteDto?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        var e = await _db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CNPJ == cnpj, cancellationToken);
        return e is null ? null : MapDto(e);
    }

    public async Task<ClienteDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var e = await _db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CodCliente == id, cancellationToken);
        return e is null ? null : MapDto(e);
    }

    public async Task<PagedResultDto<ClienteDto>> ListarAsync(int pagina, int itensPorPagina, CancellationToken cancellationToken)
    {
        var total = await _db.Clientes.CountAsync(cancellationToken);
        var items = await _db.Clientes.AsNoTracking()
            .OrderBy(c => c.CodCliente)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ClienteDto>
        {
            Items = items.Select(MapDto),
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling((double)total / itensPorPagina)
        };
    }

    public async Task<int> CriarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync(cancellationToken);
        return cliente.CodCliente;
    }

    private static ClienteDto MapDto(Cliente e) => new()
    {
        CodCliente = e.CodCliente,
        CNPJ = e.CNPJ,
        Nome = e.Nome,
        Email = e.Email,
        DataCadastro = e.DataCadastro
    };
}
