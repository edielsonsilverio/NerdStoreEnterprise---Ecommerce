using Microsoft.AspNetCore.Http;
using Polly.CircuitBreaker;
using Refit;
using System.Net;

namespace NSE.WebApp.Blazor.Extensions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CustomHttpRequestException ex)
        {
            HandleRequestExceptionAsync(httpContext, ex.StatusCode);
        }
        catch (ValidationApiException ex)
        {
            HandleRequestExceptionAsync(httpContext, ex.StatusCode);
        }
        catch (ApiException ex)
        {
            HandleRequestExceptionAsync(httpContext, ex.StatusCode);
        }
        catch (BrokenCircuitException ex)
        {
            HandleRequestBrokenCircuitExceptionAsync(httpContext);
        }
    }

    private static void HandleRequestExceptionAsync(HttpContext context, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.Unauthorized)
        {
            //Verifica o status 401 e retorna para o Login com a url que tentou acessar.
            context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}");
            return;
        }

        //Retorna o status code para ser tratado.
        context.Response.StatusCode = (int)statusCode;
    }

    private static void HandleRequestBrokenCircuitExceptionAsync(HttpContext context)
    {

        //Verifica o status 401 e retorna para o Login com a url que tentou acessar.
        context.Response.Redirect($"/sistema-indisponivel");
    }
}
