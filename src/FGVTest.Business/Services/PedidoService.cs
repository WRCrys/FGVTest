using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;
using FGVTest.Business.Exceptions;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Interfaces.Services;

namespace FGVTest.Business.Services;

/// <summary>
/// Serviço responsável pelas regras de negócio de pedidos.
/// </summary>
public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PedidoService"/>.
    /// </summary>
    public PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
    }

    /// <summary>
    /// Obtém um pedido pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do pedido encontrado.</returns>
    /// <exception cref="NotFoundException">Lançada quando o pedido não é encontrado.</exception>
    public async Task<PedidoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new NotFoundException("Pedido não encontrado.");
        return pedido;
    }

    /// <summary>
    /// Lista pedidos com paginação e filtros opcionais.
    /// </summary>
    /// <param name="pagina">Número da página.</param>
    /// <param name="itensPorPagina">Quantidade de itens por página.</param>
    /// <param name="dataInicio">Data de início do filtro.</param>
    /// <param name="dataFim">Data de fim do filtro.</param>
    /// <param name="codCliente">Código do cliente para filtrar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Resultado paginado de pedidos.</returns>
    public async Task<PagedResultDto<PedidoDto>> ListarAsync(
        int pagina,
        int itensPorPagina,
        DateTime? dataInicio,
        DateTime? dataFim,
        int? codCliente,
        CancellationToken cancellationToken)
    {
        return await _pedidoRepository.ListarAsync(pagina, itensPorPagina, dataInicio, dataFim, codCliente, cancellationToken);
    }

    /// <summary>
    /// Cria um novo pedido para o cliente identificado pelo CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados do pedido criado.</returns>
    /// <exception cref="NotFoundException">Lançada quando o cliente não é encontrado.</exception>
    public async Task<PedidoDto> CriarAsync(string cnpj, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterPorCnpjAsync(cnpj, cancellationToken);
        if (cliente is null)
            throw new NotFoundException("Cliente não encontrado.");

        var pedido = new Pedido
        {
            CodCliente = cliente.CodCliente,
            DataPedido = DateTime.UtcNow,
            ValorTotal = 0
        };

        var id = await _pedidoRepository.CriarAsync(pedido, cancellationToken);
        return await _pedidoRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException("Pedido não encontrado após criação.");
    }

    /// <summary>
    /// Adiciona um item ao pedido, decrementando o estoque do produto.
    /// </summary>
    /// <param name="codPedido">Código do pedido.</param>
    /// <param name="codProduto">Código do produto.</param>
    /// <param name="quantidade">Quantidade a adicionar.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados atualizados do pedido.</returns>
    /// <exception cref="NotFoundException">Lançada quando o pedido ou produto não é encontrado.</exception>
    /// <exception cref="ArgumentException">Lançada quando a quantidade é inválida ou o estoque é insuficiente.</exception>
    public async Task<PedidoDto> AdicionarItemAsync(int codPedido, int codProduto, int quantidade, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(codPedido, cancellationToken);
        if (pedido is null)
            throw new NotFoundException("Pedido não encontrado.");

        var produto = await _produtoRepository.ObterPorIdAsync(codProduto, cancellationToken);
        if (produto is null)
            throw new NotFoundException("Produto não encontrado.");

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (produto.Estoque < quantidade)
            throw new ArgumentException($"Estoque insuficiente para o produto '{produto.Nome}'.");

        var item = new ItensPedido
        {
            CodPedido = codPedido,
            CodProduto = codProduto,
            Quantidade = quantidade,
            PrecoUnitario = produto.Preco
        };

        await _pedidoRepository.AdicionarItemAsync(item, cancellationToken);
        await _produtoRepository.AtualizarEstoqueAsync(codProduto, produto.Estoque - quantidade, cancellationToken);

        var pedidoAtualizado = await _pedidoRepository.ObterPorIdAsync(codPedido, cancellationToken)
                               ?? throw new NotFoundException("Pedido não encontrado após adição do item.");

        var novoValorTotal = pedidoAtualizado.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        await _pedidoRepository.AtualizarValorTotalAsync(codPedido, novoValorTotal, cancellationToken);

        return await _pedidoRepository.ObterPorIdAsync(codPedido, cancellationToken)
               ?? throw new NotFoundException("Pedido não encontrado após atualização do valor total.");
    }

    /// <summary>
    /// Remove um item do pedido e devolve a quantidade ao estoque do produto.
    /// </summary>
    /// <param name="codPedido">Código do pedido.</param>
    /// <param name="codProduto">Código do produto a remover.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Dados atualizados do pedido.</returns>
    /// <exception cref="NotFoundException">Lançada quando o pedido não é encontrado.</exception>
    public async Task<PedidoDto> RemoverItemAsync(int codPedido, int codProduto, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(codPedido, cancellationToken);
        if (pedido is null)
            throw new NotFoundException("Pedido não encontrado.");

        var item = pedido.Itens.FirstOrDefault(i => i.CodProduto == codProduto);
        if (item is not null)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(codProduto, cancellationToken);
            if (produto is not null)
                await _produtoRepository.AtualizarEstoqueAsync(codProduto, produto.Estoque + item.Quantidade, cancellationToken);
        }

        await _pedidoRepository.RemoverItemAsync(codPedido, codProduto, cancellationToken);

        var pedidoAtualizado = await _pedidoRepository.ObterPorIdAsync(codPedido, cancellationToken)
                               ?? throw new NotFoundException("Pedido não encontrado após remoção do item.");

        var novoValorTotal = pedidoAtualizado.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        await _pedidoRepository.AtualizarValorTotalAsync(codPedido, novoValorTotal, cancellationToken);

        return await _pedidoRepository.ObterPorIdAsync(codPedido, cancellationToken)
               ?? throw new NotFoundException("Pedido não encontrado após atualização do valor total.");
    }
}
