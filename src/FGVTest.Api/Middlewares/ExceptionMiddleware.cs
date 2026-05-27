using System.Text.Json;
using FGVTest.Business.Exceptions;

namespace FGVTest.Api.Middlewares;

/// <summary>
/// Middleware global de tratamento de exceções.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ExceptionMiddleware"/>.
    /// </summary>
    /// <param name="next">Próximo middleware no pipeline.</param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Intercepta exceções e retorna respostas HTTP padronizadas.
    /// </summary>
    /// <param name="context">Contexto HTTP da requisição.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (Exception)
        {
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError,
                "Ocorreu um erro interno no servidor.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string mensagem)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new { mensagem });
        await context.Response.WriteAsync(body, context.RequestAborted);
    }
}
