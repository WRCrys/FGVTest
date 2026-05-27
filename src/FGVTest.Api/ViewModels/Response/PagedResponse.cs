namespace FGVTest.Api.ViewModels.Response;

/// <summary>
/// Envelope de resposta paginada.
/// </summary>
/// <typeparam name="T">Tipo dos itens da lista.</typeparam>
public class PagedResponse<T>
{
    /// <summary>Itens da página atual.</summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>Número da página atual.</summary>
    public int PaginaAtual { get; set; }

    /// <summary>Total de páginas.</summary>
    public int TotalPaginas { get; set; }

    /// <summary>Total de itens.</summary>
    public int TotalItens { get; set; }

    /// <summary>Quantidade de itens por página.</summary>
    public int ItensPorPagina { get; set; }
}
