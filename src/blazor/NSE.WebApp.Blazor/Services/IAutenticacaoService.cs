using NSE.WebApp.Blazor.Models;

namespace NSE.WebApp.Blazor.Services;

public interface IAutenticacaoService
{
    Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin);

    Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro);
}