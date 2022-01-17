using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Services;
public interface IAutenticacaoService
{
    Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin);
    Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro);
    Task RealizarLogin(UsuarioRespostaLogin resposta);
    Task Logout();
    bool TokenExpirado();
    Task<bool> RefreshTokenValido();
}

