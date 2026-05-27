using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FGVTest.Api.ViewModels.Request;
using FGVTest.Api.ViewModels.Response;

namespace FGVTest.Test.E2E;

public class ProdutosE2ETests : IClassFixture<SalesApiFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ProdutosE2ETests(SalesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_Produtos_RetornaCriado201()
    {
        var request = new CriarProdutoRequest { Nome = "Caneta Azul", Preco = 3.50m, Estoque = 100 };

        var response = await _client.PostAsJsonAsync("/api/produtos", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<ProdutoResponse>(JsonOptions);
        body!.Nome.Should().Be("Caneta Azul");
        body.Preco.Should().Be(3.50m);
        body.Estoque.Should().Be(100);
    }

    [Fact]
    public async Task POST_Produtos_ComPrecoZero_RetornaBadRequest400()
    {
        var request = new CriarProdutoRequest { Nome = "Produto Invalido", Preco = 0, Estoque = 10 };

        var response = await _client.PostAsJsonAsync("/api/produtos", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Produtos_ComNomeVazio_RetornaBadRequest400()
    {
        var request = new CriarProdutoRequest { Nome = "", Preco = 5m, Estoque = 10 };

        var response = await _client.PostAsJsonAsync("/api/produtos", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_Produtos_PorId_QuandoExiste_RetornaProduto()
    {
        var created = new CriarProdutoRequest { Nome = "Lápis", Preco = 1.00m, Estoque = 50 };
        var postResp = await _client.PostAsJsonAsync("/api/produtos", created);
        var produto = await postResp.Content.ReadFromJsonAsync<ProdutoResponse>(JsonOptions);

        var response = await _client.GetAsync($"/api/produtos/{produto!.CodProduto}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ProdutoResponse>(JsonOptions);
        body!.Nome.Should().Be("Lápis");
    }

    [Fact]
    public async Task GET_Produtos_PorId_QuandoNaoExiste_RetornaNotFound404()
    {
        var response = await _client.GetAsync("/api/produtos/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_Produtos_RetornaListaPaginada()
    {
        var response = await _client.GetAsync("/api/produtos?pagina=1&itensPorPagina=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResponse<ProdutoResponse>>(JsonOptions);
        body!.PaginaAtual.Should().Be(1);
    }
}
