using FluentAssertions;
using FGVTest.Business.DTOs;
using FGVTest.Business.Exceptions;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Services;
using Moq;

namespace FGVTest.Test.Unit;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();
    private readonly Mock<IClienteRepository> _clienteRepoMock = new();
    private readonly Mock<IProdutoRepository> _produtoRepoMock = new();
    private readonly PedidoService _sut;

    public PedidoServiceTests()
    {
        _sut = new PedidoService(_pedidoRepoMock.Object, _clienteRepoMock.Object, _produtoRepoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_QuandoClienteExiste_CriaPedidoComValorZero()
    {
        _clienteRepoMock.Setup(r => r.ObterPorCnpjAsync("12345678000100", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ClienteDto { CodCliente = 1, CNPJ = "12345678000100" });

        _pedidoRepoMock.Setup(r => r.CriarAsync(It.IsAny<FGVTest.Business.Entities.Pedido>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(10);

        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new PedidoDto
                       {
                           CodPedido = 10,
                           CodCliente = 1,
                           ValorTotal = 0,
                           Cliente = new ClienteDto { CodCliente = 1 },
                           Itens = []
                       });

        var result = await _sut.CriarAsync("12345678000100", CancellationToken.None);

        result.ValorTotal.Should().Be(0);
        result.CodPedido.Should().Be(10);

        _pedidoRepoMock.Verify(r => r.CriarAsync(
            It.Is<FGVTest.Business.Entities.Pedido>(p => p.ValorTotal == 0 && p.CodCliente == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_QuandoClienteNaoExiste_LancaNotFoundException()
    {
        _clienteRepoMock.Setup(r => r.ObterPorCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((ClienteDto?)null);

        var act = () => _sut.CriarAsync("00000000000000", CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Cliente não encontrado.");
    }

    [Fact]
    public async Task AdicionarItemAsync_ComEstoqueInsuficiente_LancaArgumentException()
    {
        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new PedidoDto { CodPedido = 1, Cliente = new ClienteDto(), Itens = [] });

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ProdutoDto { CodProduto = 5, Nome = "Caneta", Preco = 2m, Estoque = 3 });

        var act = () => _sut.AdicionarItemAsync(1, 5, 10, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Estoque insuficiente para o produto 'Caneta'.");
    }

    [Fact]
    public async Task AdicionarItemAsync_ComQuantidadeZero_LancaArgumentException()
    {
        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new PedidoDto { CodPedido = 1, Cliente = new ClienteDto(), Itens = [] });

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ProdutoDto { CodProduto = 5, Nome = "Caneta", Preco = 2m, Estoque = 10 });

        var act = () => _sut.AdicionarItemAsync(1, 5, 0, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("A quantidade deve ser maior que zero.");
    }

    [Fact]
    public async Task AdicionarItemAsync_ComDadosValidos_AtualizaEstoqueEValorTotal()
    {
        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new PedidoDto
                       {
                           CodPedido = 1,
                           Cliente = new ClienteDto(),
                           Itens = [new ItensPedidoDto { CodProduto = 5, Quantidade = 2, PrecoUnitario = 10m, Produto = new ProdutoDto() }]
                       });

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ProdutoDto { CodProduto = 5, Nome = "Caneta", Preco = 10m, Estoque = 20 });

        _pedidoRepoMock.Setup(r => r.AdicionarItemAsync(It.IsAny<FGVTest.Business.Entities.ItensPedido>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        _produtoRepoMock.Setup(r => r.AtualizarEstoqueAsync(5, 18, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        _pedidoRepoMock.Setup(r => r.AtualizarValorTotalAsync(1, 20m, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        _pedidoRepoMock.SetupSequence(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new PedidoDto { CodPedido = 1, Cliente = new ClienteDto(), Itens = [] })
                       .ReturnsAsync(new PedidoDto
                       {
                           CodPedido = 1,
                           Cliente = new ClienteDto(),
                           Itens = [new ItensPedidoDto { CodProduto = 5, Quantidade = 2, PrecoUnitario = 10m, Produto = new ProdutoDto() }]
                       })
                       .ReturnsAsync(new PedidoDto
                       {
                           CodPedido = 1,
                           ValorTotal = 20m,
                           Cliente = new ClienteDto(),
                           Itens = [new ItensPedidoDto { CodProduto = 5, Quantidade = 2, PrecoUnitario = 10m, Produto = new ProdutoDto() }]
                       });

        var result = await _sut.AdicionarItemAsync(1, 5, 2, CancellationToken.None);

        result.ValorTotal.Should().Be(20m);
        _produtoRepoMock.Verify(r => r.AtualizarEstoqueAsync(5, 18, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AdicionarItemAsync_QuandoPedidoNaoExiste_LancaNotFoundException()
    {
        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((PedidoDto?)null);

        var act = () => _sut.AdicionarItemAsync(99, 1, 1, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Pedido não encontrado.");
    }

    [Fact]
    public async Task RemoverItemAsync_QuandoPedidoNaoExiste_LancaNotFoundException()
    {
        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((PedidoDto?)null);

        var act = () => _sut.RemoverItemAsync(99, 1, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Pedido não encontrado.");
    }

    [Fact]
    public async Task RemoverItemAsync_DevolveProdutoAoEstoque()
    {
        _pedidoRepoMock.SetupSequence(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new PedidoDto
                       {
                           CodPedido = 1,
                           Cliente = new ClienteDto(),
                           Itens = [new ItensPedidoDto { CodProduto = 5, Quantidade = 3, PrecoUnitario = 5m, Produto = new ProdutoDto() }]
                       })
                       .ReturnsAsync(new PedidoDto { CodPedido = 1, Cliente = new ClienteDto(), Itens = [] })
                       .ReturnsAsync(new PedidoDto { CodPedido = 1, ValorTotal = 0, Cliente = new ClienteDto(), Itens = [] });

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ProdutoDto { CodProduto = 5, Estoque = 7 });

        _pedidoRepoMock.Setup(r => r.RemoverItemAsync(1, 5, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        _produtoRepoMock.Setup(r => r.AtualizarEstoqueAsync(5, 10, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        _pedidoRepoMock.Setup(r => r.AtualizarValorTotalAsync(1, 0m, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        await _sut.RemoverItemAsync(1, 5, CancellationToken.None);

        _produtoRepoMock.Verify(r => r.AtualizarEstoqueAsync(5, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
