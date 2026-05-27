namespace FGVTest.Business.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }
    public int ItensPorPagina { get; set; }
}
