using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NSE.WebAPI.Core;

public interface IStartup
{
    IConfiguration Configuration { get; }
    void Configure(WebApplication app, IWebHostEnvironment env);
    void ConfigureServices(IServiceCollection services);
}
