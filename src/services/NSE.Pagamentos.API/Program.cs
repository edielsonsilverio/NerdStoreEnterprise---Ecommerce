using Microsoft.AspNetCore.Builder;
using NSE.Pagamento.API;
using NSE.WebAPI.Core;
 

var builder = WebApplication.CreateBuilder(args).UseStartup<Startup>();