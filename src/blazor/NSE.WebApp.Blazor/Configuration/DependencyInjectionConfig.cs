using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NSE.WebApp.Blazor.Services;
using NSE.WebApp.Blazor.Services.Handlers;
using NSE.WebApp.Blazor.Extensions;
using Polly.Retry;
using Polly.Extensions.Http;
using Polly;

namespace NSE.WebApp.Blazor.Configuration;
public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, 
                                        IConfiguration configuration,
                                        IWebAssemblyHostEnvironment hostEnvironment)
    {
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress) });


        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();

        services.AddHttpClient<ICatalogoService, CatalogoService>()
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
               //.AddTransientHttpErrorPolicy(
               //p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
               //.AddPolicyHandler(PollyExtensions.EsperarTentar())
               //.AddTransientHttpErrorPolicy(
               //    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
               ;



        services.AddHttpContextAccessor();
        services.AddScoped<HttpContextAccessor>();
        services.AddScoped<IUser, AspNetUser>();

        #region Refit

        //services.AddHttpClient("Refit",
        //        options =>
        //        {
        //            options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
        //        })
        //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        //.AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);

        #endregion
    }
    public class PollyExtensions
    {
        //Método usuando a biblioteca Polly, serve para manipular o request em caso de erro.
        public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
        {
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tentando pela {retryCount} vez!");
                    Console.ForegroundColor = ConsoleColor.White;
                });

            return retry;
        }
    }
}