using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;

namespace Supermercado.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Deixa a requisição seguir o fluxo normal
            await _next(context);
        }
        catch (Exception ex)
        {
            // Se houver qualquer erro não tratado, intercepta aqui
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = context.Response;
        var status = HttpStatusCode.InternalServerError;
        var mensagem = "Ocorreu um erro interno no servidor.";

        // Mapeia exceções de regras de negócio para HTTP 400 Bad Request
        if (exception is InvalidOperationException || exception is ArgumentException)
        {
            status = HttpStatusCode.BadRequest;
            mensagem = exception.Message;
        }

        response.StatusCode = (int)status;

        var resultado = JsonSerializer.Serialize(new
        {
            status = response.StatusCode,
            mensagem = mensagem,
            dataHora = DateTime.UtcNow
        });

        return response.WriteAsync(resultado);
    }
}