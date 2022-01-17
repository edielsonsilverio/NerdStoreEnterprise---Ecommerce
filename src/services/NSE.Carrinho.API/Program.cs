using Microsoft.AspNetCore.Builder;
using NSE.Carrinho.API;
using NSE.WebAPI.Core;

var builder = WebApplication.CreateBuilder(args).UseStartup<Startup>();