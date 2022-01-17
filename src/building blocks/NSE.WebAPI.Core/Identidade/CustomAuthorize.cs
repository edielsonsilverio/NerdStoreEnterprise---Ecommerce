using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace NSE.WebAPI.Core.Identidade;

public class CustomAuthorization
{
    //Método para verificar se o usuário está autenticado e se tem as permissões solicitadas.
    public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
    {
        return context.User.Identity.IsAuthenticated &&
               context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
    }

    public static bool ValidarRoleUsuario(HttpContext context, string role)
    {
        return context.User.Identity.IsAuthenticated &&
               context.User.IsInRole(role);
    }
}

//Classe para criar um filtro [atributo]
public class ClaimsAuthorizeAttribute : TypeFilterAttribute
{
    public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))
    {
        Arguments = new object[] { new Claim(claimName, claimValue) };
    }
}


//Classe que implementa o filtro de autorização do identity
public class RequisitoClaimFilter : IAuthorizationFilter
{
    private readonly Claim _claim;

    //Recebe a Claim por parâmetro
    public RequisitoClaimFilter(Claim claim) => _claim = claim;

    //Método para verificar se os dados de autênticação são válidos.
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //Verifica se o usuário está autenticado.
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(401);
            return;
        }

        //Verifica se o usuário está autenticação e se ele tem permissão.
        //Ex: Está logado, mas não tem claim solicidada => [ClaimsAuthorize("Catalogo","Ler")]
        if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            context.Result = new StatusCodeResult(403);

    }
}
