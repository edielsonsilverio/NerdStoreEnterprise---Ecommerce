using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Security.JwtExtensions;

namespace NSE.WebAPI.Core.Identidade;

public static class JwtConfig
{
    public static void AddJwtConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {

        //Obter as informações da section [AppSettings] que fica no arquivo appSettings.{Ambiente de Desenvolvimento}.json
        var appSettingsSection = configuration.GetSection("AppSettings");

        //Configura as informações no pipeline.
        services.Configure<AppSettings>(appSettingsSection);

        //Obtêm os dados populados.
        var appSettings = appSettingsSection.Get<AppSettings>();

        //Configuração da autenticação do via JWT.
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        //Configuração dos parâmetros do token
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;  //Acesso pelo https
            x.SaveToken = true;             //Permitir salvar o token na instância
            x.BackchannelHttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } };
            x.SetJwksOptions(new JwkOptions(appSettings.AutenticacaoJwksUrl));
        });
    }

    public static void UseAuthConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}