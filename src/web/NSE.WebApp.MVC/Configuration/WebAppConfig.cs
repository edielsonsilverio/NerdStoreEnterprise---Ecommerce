using Microsoft.AspNetCore.Localization;
using NSE.WebApp.MVC.Extensions;
using System.Globalization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

namespace NSE.WebApp.MVC.Configuration;
public static class WebAppConfig
{
    public static void AddMvcConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews().AddRazorRuntimeCompilation();

        services.AddDataProtection()
            .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/var/data_protection_keys/"))
            .SetApplicationName("NerdStoreEnterprise");

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.Configure<AppSettings>(configuration);
    }
    public static void UseMvcConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();
        //if (!env.IsDevelopment())
        //{
        //    //Middware que captura os erros não tratados
        //    app.UseExceptionHandler("/erro/500");

        //    //Captura os erros que retornaram algum código de erro.
        //    app.UseStatusCodePagesWithRedirects("/erro/{0}");
        //    app.UseHsts();
        //}
        //else
        //    app.UseDeveloperExceptionPage();

        //Middware que captura os erros não tratados
        app.UseExceptionHandler("/erro/500");

        //Captura os erros que retornaram algum código de erro.
        app.UseStatusCodePagesWithRedirects("/erro/{0}");
        app.UseHsts();


        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        //Está opção deve ficar entre o UseRouting e o UseEndPoint
        app.UseIdentityConfiguration();


        //Configura a cultura do Browser e da tela
        var suportedCultures = new[] { new CultureInfo("pt-BR") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new  RequestCulture("pt-BR"),
            SupportedCultures = suportedCultures,
            SupportedUICultures = suportedCultures
        });

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Catalogo}/{action=Index}/{id?}");
        });
    }
}
