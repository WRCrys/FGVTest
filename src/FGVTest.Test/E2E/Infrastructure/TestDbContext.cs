using FGVTest.Business.Entities;
using Microsoft.EntityFrameworkCore;

namespace FGVTest.Test.E2E.Infrastructure;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItensPedido> ItensPedido => Set<ItensPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasKey(c => c.CodCliente);
            e.Property(c => c.CodCliente).ValueGeneratedOnAdd();
            e.HasIndex(c => c.CNPJ).IsUnique();
        });

        modelBuilder.Entity<Produto>(e =>
        {
            e.HasKey(p => p.CodProduto);
            e.Property(p => p.CodProduto).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.HasKey(p => p.CodPedido);
            e.Property(p => p.CodPedido).ValueGeneratedOnAdd();
            e.Ignore(p => p.Cliente);
            e.Ignore(p => p.Itens);
        });

        modelBuilder.Entity<ItensPedido>(e =>
        {
            e.HasKey(i => new { i.CodPedido, i.CodProduto });
            e.Ignore(i => i.Produto);
        });
    }
}
