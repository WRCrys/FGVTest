using FluentAssertions;
using FGVTest.Business.DTOs;
using FGVTest.Business.Exceptions;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Services;
using Moq;

namespace FGVTest.Test.Unit;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repoMock = new();
    private readonly ClienteService _sut;

    public ClienteServiceTests()
    {
        _sut = new ClienteService(_repoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_QuandoCnpjNaoExiste_RetornaClienteDto()
    {
        _repoMock.Setup(r => r.ObterPorCnpjAsync("12345678000100", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((ClienteDto?)null);

        _repoMock.Setup(r => r.CriarAsync(It.IsAny<FGVTest.Business.Entities.Cliente>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(1);

        _repoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ClienteDto
                 {
                     CodCliente = 1,
                     CNPJ = "12345678000100",
                     Nome = "Empresa Teste",
                     Email = "teste@empresa.com",
                     DataCadastro = DateTime.UtcNow
                 });

        var result = await _sut.CriarAsync("12345678000100", "Empresa Teste", "teste@empresa.com", CancellationToken.None);

        result.Should().NotBeNull();
        result.CNPJ.Should().Be("12345678000100");
        result.Nome.Should().Be("Empresa Teste");
    }

    [Fact]
    public async Task CriarAsync_QuandoCnpjJaExiste_LancaArgumentException()
    {
        _repoMock.Setup(r => r.ObterPorCnpjAsync("12345678000100", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ClienteDto { CNPJ = "12345678000100" });

        var act = () => _sut.CriarAsync("12345678000100", "Outro", "outro@email.com", CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Já existe um cliente cadastrado com este CNPJ.");
    }

    [Fact]
    public async Task ObterPorCnpjAsync_QuandoClienteExiste_RetornaDto()
    {
        var dto = new ClienteDto { CodCliente = 1, CNPJ = "12345678000100" };
        _repoMock.Setup(r => r.ObterPorCnpjAsync("12345678000100", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dto);

        var result = await _sut.ObterPorCnpjAsync("12345678000100", CancellationToken.None);

        result.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task ObterPorCnpjAsync_QuandoClienteNaoExiste_LancaNotFoundException()
    {
        _repoMock.Setup(r => r.ObterPorCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((ClienteDto?)null);

        var act = () => _sut.ObterPorCnpjAsync("00000000000000", CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Cliente não encontrado.");
    }

    [Fact]
    public async Task ListarAsync_RetornaPagedResult()
    {
        var paged = new PagedResultDto<ClienteDto>
        {
            Items = [new ClienteDto { CodCliente = 1 }],
            PaginaAtual = 1,
            ItensPorPagina = 10,
            TotalItens = 1,
            TotalPaginas = 1
        };

        _repoMock.Setup(r => r.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(paged);

        var result = await _sut.ListarAsync(1, 10, CancellationToken.None);

        result.TotalItens.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }
}
