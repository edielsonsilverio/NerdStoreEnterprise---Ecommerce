using NSE.WebAPI.Core;
using NSE.WebApp.MVC;

var builder = WebApplication.CreateBuilder(args).UseStartup<Startup>();


//var builder = WebApplication.CreateBuilder(args);
//var startup = new Startup(builder.Environment);
//startup.ConfigureServices(builder.Services);

//var app = builder.Build();
//startup.Configure(app, app.Environment);
//app.Run();