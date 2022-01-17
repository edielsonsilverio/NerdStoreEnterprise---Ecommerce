using Polly.CircuitBreaker;
using Refit;
using System.Net;
using NSE.WebApp.MVC.Services;
using Grpc.Core;

namespace NSE.WebApp.MVC.Extensions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private static IAutenticacaoService _autenticacaoService;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IAutenticacaoService autenticacaoService)
    {
        //A injeção tem que ser feita dentro do método invoke, pois o middle é singleton
        // e a autentication é scope, por isso não pode ser injetado no construtor.
        _autenticacaoService = autenticacaoService;
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
            var erro = ex.Message;
            HandleRequestBrokenCircuitExceptionAsync(httpContext);
        }
        catch (RpcException ex)
        {
            //400 Bad Request	    INTERNAL
            //401 Unauthorized      UNAUTHENTICATED
            //403 Forbidden         PERMISSION_DENIED
            //404 Not Found         UNIMPLEMENTED

            var statusCode = ex.StatusCode switch
            {
                StatusCode.Internal => 400,
                StatusCode.Unauthenticated => 401,
                StatusCode.PermissionDenied => 403,
                StatusCode.Unimplemented => 404,
                _ => 500
            };

            var httpStatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCode.ToString());

            HandleRequestExceptionAsync(httpContext, httpStatusCode);
        }
    }

    private static void HandleRequestExceptionAsync(HttpContext context, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.Unauthorized)
        {
            if (_autenticacaoService.TokenExpirado())
            {
                if (_autenticacaoService.RefreshTokenValido().Result)
                {
                    context.Response.Redirect(context.Request.Path);
                    return;
                }
            }

            _autenticacaoService.Logout();

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

