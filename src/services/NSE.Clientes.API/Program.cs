using Microsoft.AspNetCore.Builder;
using NSE.Clientes.API;
using NSE.WebAPI.Core;

var builder = WebApplication.CreateBuilder(args).UseStartup<Startup>();