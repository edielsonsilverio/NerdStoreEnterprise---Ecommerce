using NSE.WebApp.Blazor.Extensions;
using System.Net.Http.Headers;

namespace NSE.WebApp.Blazor.Services.Handlers;
public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IUser _user;
    public HttpClientAuthorizationDelegatingHandler(IUser user)
    {
        _user = user;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Obtêm os dados do Header
        var authorizationHeader = _user.ObterHttpContext().Request.Headers["Authorization"];

        //Se encontrar insere header do request
        if (!string.IsNullOrEmpty(authorizationHeader))
            request.Headers.Add("Authorization", new List<string>() { authorizationHeader });

        //Obtêm o token do usuário logado
        var token = _user.ObterUserToken();

        //Se encontrar um token, insere no header do request
        if (token != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}