namespace FGVTest.Api.ViewModels.Request;

/// <summary>
/// Parâmetros de paginação.
/// </summary>
public class PaginacaoRequest
{
    /// <summary>Número da página (começa em 1).</summary>
    public int Pagina { get; set; } = 1;

    /// <summary>Quantidade de itens por página.</summary>
    public int ItensPorPagina { get; set; } = 10;
}
