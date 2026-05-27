using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FGVTest.Api.ViewModels.Request;
using FGVTest.Api.ViewModels.Response;

namespace FGVTest.Test.E2E;

public class PedidosE2ETests : IClassFixture<SalesApiFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public PedidosE2ETests(SalesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<ClienteResponse> CriarClienteAsync(string cnpj)
    {
        var request = new CriarClienteRequest { CNPJ = cnpj, Nome = "Cliente Pedido", Email = "pedido@test.com" };
        var resp = await _client.PostAsJsonAsync("/api/clientes", request);
        return (await resp.Content.ReadFromJsonAsync<ClienteResponse>(JsonOptions))!;
    }

    private async Task<ProdutoResponse> CriarProdutoAsync(string nome, decimal preco, int estoque)
    {
        var request = new CriarProdutoRequest { Nome = nome, Preco = preco, Estoque = estoque };
        var resp = await _client.PostAsJsonAsync("/api/produtos", request);
        return (await resp.Content.ReadFromJsonAsync<ProdutoResponse>(JsonOptions))!;
    }

    [Fact]
    public async Task POST_Pedidos_QuandoClienteExiste_RetornaCriado201()
    {
        await CriarClienteAsync("55666777000144");

        var request = new CriarPedidoRequest { CNPJ = "55666777000144" };
        var response = await _client.PostAsJsonAsync("/api/pedidos", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions);
        body!.ValorTotal.Should().Be(0);
        body.Itens.Should().BeEmpty();
    }

    [Fact]
    public async Task POST_Pedidos_QuandoClienteNaoExiste_RetornaNotFound404()
    {
        var request = new CriarPedidoRequest { CNPJ = "00000000000099" };

        var response = await _client.PostAsJsonAsync("/api/pedidos", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_Pedidos_AdicionarItem_AtualizaValorTotal()
    {
        await CriarClienteAsync("66777888000133");
        var produto = await CriarProdutoAsync("Caneta E2E", 5.00m, 50);

        var pedidoResp = await _client.PostAsJsonAsync("/api/pedidos", new CriarPedidoRequest { CNPJ = "66777888000133" });
        var pedido = (await pedidoResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;

        var itemRequest = new AdicionarItemRequest { CodProduto = produto.CodProduto, Quantidade = 3 };
        var addResp = await _client.PostAsJsonAsync($"/api/pedidos/{pedido.CodPedido}/itens", itemRequest);

        addResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = (await addResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;
        updated.ValorTotal.Should().Be(15.00m);
        updated.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task POST_Pedidos_AdicionarItem_ComEstoqueInsuficiente_RetornaBadRequest400()
    {
        await CriarClienteAsync("77888999000122");
        var produto = await CriarProdutoAsync("Produto Escasso", 10.00m, 2);

        var pedidoResp = await _client.PostAsJsonAsync("/api/pedidos", new CriarPedidoRequest { CNPJ = "77888999000122" });
        var pedido = (await pedidoResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;

        var itemRequest = new AdicionarItemRequest { CodProduto = produto.CodProduto, Quantidade = 10 };
        var addResp = await _client.PostAsJsonAsync($"/api/pedidos/{pedido.CodPedido}/itens", itemRequest);

        addResp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DELETE_Pedidos_RemoverItem_DevolveProdutoAoEstoque()
    {
        await CriarClienteAsync("88999000000111");
        var produto = await CriarProdutoAsync("Produto Remover", 8.00m, 10);

        var pedidoResp = await _client.PostAsJsonAsync("/api/pedidos", new CriarPedidoRequest { CNPJ = "88999000000111" });
        var pedido = (await pedidoResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;

        await _client.PostAsJsonAsync($"/api/pedidos/{pedido.CodPedido}/itens",
            new AdicionarItemRequest { CodProduto = produto.CodProduto, Quantidade = 3 });

        var delResp = await _client.DeleteAsync($"/api/pedidos/{pedido.CodPedido}/itens/{produto.CodProduto}");

        delResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = (await delResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;
        updated.ValorTotal.Should().Be(0);
        updated.Itens.Should().BeEmpty();

        var produtoAtualizado = (await (await _client.GetAsync($"/api/produtos/{produto.CodProduto}"))
            .Content.ReadFromJsonAsync<ProdutoResponse>(JsonOptions))!;
        produtoAtualizado.Estoque.Should().Be(10);
    }

    [Fact]
    public async Task GET_Pedidos_PorId_QuandoExiste_RetornaPedido()
    {
        await CriarClienteAsync("99000111000100");
        var pedidoResp = await _client.PostAsJsonAsync("/api/pedidos", new CriarPedidoRequest { CNPJ = "99000111000100" });
        var pedido = (await pedidoResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;

        var response = await _client.GetAsync($"/api/pedidos/{pedido.CodPedido}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = (await response.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;
        body.CodPedido.Should().Be(pedido.CodPedido);
    }

    [Fact]
    public async Task GET_Pedidos_PorId_QuandoNaoExiste_RetornaNotFound404()
    {
        var response = await _client.GetAsync("/api/pedidos/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_Pedidos_ComFiltroCliente_RetornaSomentePedidosDoCliente()
    {
        await CriarClienteAsync("10111222000189");
        var pedidoResp = await _client.PostAsJsonAsync("/api/pedidos", new CriarPedidoRequest { CNPJ = "10111222000189" });
        var pedido = (await pedidoResp.Content.ReadFromJsonAsync<PedidoResponse>(JsonOptions))!;

        var codCliente = pedido.Cliente.CodCliente;
        var response = await _client.GetAsync($"/api/pedidos?pagina=1&itensPorPagina=10&codCliente={codCliente}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = (await response.Content.ReadFromJsonAsync<PagedResponse<PedidoResponse>>(JsonOptions))!;
        body.Items.Should().OnlyContain(p => p.Cliente.CodCliente == codCliente);
    }
}
