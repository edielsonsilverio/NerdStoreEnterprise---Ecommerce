using Microsoft.Extensions.Configuration;

namespace NSE.Core.Utils;
public static class ConfigurationExtensions
{

    //Cria um método de extensão para pegar as informações do app.settings.
    public static string GetMessageQueueConnection(this IConfiguration configuration, string name)
    {
        return configuration?.GetSection("MessageQueueConnection")?[name];
    }
}
