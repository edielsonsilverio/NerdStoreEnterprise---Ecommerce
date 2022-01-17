using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NSE.WebApp.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var startup = new Startup();
startup.ConfigureServices(builder.Services, builder.HostEnvironment);
await builder.Build().RunAsync();
