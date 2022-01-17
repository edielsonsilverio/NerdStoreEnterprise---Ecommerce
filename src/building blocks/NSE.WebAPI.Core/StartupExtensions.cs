using Microsoft.AspNetCore.Builder;

namespace NSE.WebAPI.Core;

public static class StartupExtensions
{
    public static WebApplicationBuilder UseStartup<TStartup>(this WebApplicationBuilder webAppBuilder) where TStartup : IStartup
    {
        var startup = Activator.CreateInstance(typeof(TStartup), webAppBuilder.Environment) as IStartup;
        if (startup == null) throw new ArgumentException("Classe Startup.cs inválida!");

        startup.ConfigureServices(webAppBuilder.Services);

        var app = webAppBuilder.Build();
        startup.Configure(app, app.Environment);
        app.Run();

        return webAppBuilder;
    }
}
