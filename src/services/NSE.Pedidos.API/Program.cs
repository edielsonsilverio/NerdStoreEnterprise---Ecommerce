using Microsoft.AspNetCore.Builder;
using NSE.Pedidos.API;
using NSE.WebAPI.Core;

var builder = WebApplication.CreateBuilder(args).UseStartup<Startup>();