using FluentAssertions;
using FGVTest.Business.DTOs;
using FGVTest.Business.Exceptions;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Services;
using Moq;

namespace FGVTest.Test.Unit;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _repoMock = new();
    private readonly ProdutoService _sut;

    public ProdutoServiceTests()
    {
        _sut = new ProdutoService(_repoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_RetornaProdutoDto()
    {
        _repoMock.Setup(r => r.CriarAsync(It.IsAny<FGVTest.Business.Entities.Produto>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(1);

        _repoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ProdutoDto { CodProduto = 1, Nome = "Caneta", Preco = 2.50m, Estoque = 100 });

        var result = await _sut.CriarAsync("Caneta", 2.50m, 100, CancellationToken.None);

        result.Should().NotBeNull();
        result.Nome.Should().Be("Caneta");
        result.Preco.Should().Be(2.50m);
    }

    [Theory]
    [InlineData("", 10, 5)]
    [InlineData("  ", 10, 5)]
    public async Task CriarAsync_ComNomeVazio_LancaArgumentException(string nome, decimal preco, int estoque)
    {
        var act = () => _sut.CriarAsync(nome, preco, estoque, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O nome do produto não pode ser vazio.");
    }

    [Fact]
    public async Task CriarAsync_ComPrecoZero_LancaArgumentException()
    {
        var act = () => _sut.CriarAsync("Produto", 0, 10, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O preço do produto deve ser maior que zero.");
    }

    [Fact]
    public async Task CriarAsync_ComPrecoNegativo_LancaArgumentException()
    {
        var act = () => _sut.CriarAsync("Produto", -1m, 10, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O preço do produto deve ser maior que zero.");
    }

    [Fact]
    public async Task CriarAsync_ComEstoqueNegativo_LancaArgumentException()
    {
        var act = () => _sut.CriarAsync("Produto", 10m, -1, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O estoque do produto não pode ser negativo.");
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoProdutoNaoExiste_LancaNotFoundException()
    {
        _repoMock.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((ProdutoDto?)null);

        var act = () => _sut.ObterPorIdAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Produto não encontrado.");
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoProdutoExiste_RetornaDto()
    {
        var dto = new ProdutoDto { CodProduto = 5, Nome = "Lápis", Preco = 1.0m, Estoque = 50 };
        _repoMock.Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dto);

        var result = await _sut.ObterPorIdAsync(5, CancellationToken.None);

        result.Should().BeEquivalentTo(dto);
    }
}
