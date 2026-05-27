namespace FGVTest.Business.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado no banco de dados.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância de <see cref="NotFoundException"/>.
    /// </summary>
    /// <param name="mensagem">Mensagem descritiva do recurso não encontrado.</param>
    public NotFoundException(string mensagem) : base(mensagem) { }
}
