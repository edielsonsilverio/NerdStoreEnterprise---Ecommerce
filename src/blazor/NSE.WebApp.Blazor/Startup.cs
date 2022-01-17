using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace NSE.WebApp.Blazor;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
    {
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress) });
    }
}
