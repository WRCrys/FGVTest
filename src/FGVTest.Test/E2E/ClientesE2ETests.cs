using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FGVTest.Api.ViewModels.Request;
using FGVTest.Api.ViewModels.Response;

namespace FGVTest.Test.E2E;

public class ClientesE2ETests : IClassFixture<SalesApiFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ClientesE2ETests(SalesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_Clientes_RetornaCriado201()
    {
        var request = new CriarClienteRequest
        {
            CNPJ = "11222333000181",
            Nome = "Empresa E2E",
            Email = "e2e@empresa.com"
        };

        var response = await _client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<ClienteResponse>(JsonOptions);
        body!.CNPJ.Should().Be("11222333000181");
        body.Nome.Should().Be("Empresa E2E");
    }

    [Fact]
    public async Task POST_Clientes_ComCnpjDuplicado_RetornaBadRequest400()
    {
        var request = new CriarClienteRequest
        {
            CNPJ = "22333444000155",
            Nome = "Empresa Duplicada",
            Email = "dup@empresa.com"
        };

        await _client.PostAsJsonAsync("/api/clientes", request);
        var response = await _client.PostAsJsonAsync("/api/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_Clientes_RetornaListaPaginada()
    {
        var response = await _client.GetAsync("/api/clientes?pagina=1&itensPorPagina=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResponse<ClienteResponse>>(JsonOptions);
        body.Should().NotBeNull();
        body!.PaginaAtual.Should().Be(1);
    }

    [Fact]
    public async Task GET_Clientes_PorCnpj_QuandoExiste_RetornaCliente()
    {
        var created = new CriarClienteRequest
        {
            CNPJ = "33444555000177",
            Nome = "Empresa GetCnpj",
            Email = "getcnpj@empresa.com"
        };
        await _client.PostAsJsonAsync("/api/clientes", created);

        var response = await _client.GetAsync($"/api/clientes/cnpj/{created.CNPJ}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ClienteResponse>(JsonOptions);
        body!.CNPJ.Should().Be(created.CNPJ);
    }

    [Fact]
    public async Task GET_Clientes_PorCnpj_QuandoNaoExiste_RetornaNotFound404()
    {
        var response = await _client.GetAsync("/api/clientes/cnpj/00000000000000");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
