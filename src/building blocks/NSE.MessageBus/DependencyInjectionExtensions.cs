using Microsoft.Extensions.DependencyInjection;

namespace NSE.MessageBus;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, string connection)
    {
        //Se a conexão for nula, então retorna uma exceção.
        if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException();

        services.AddSingleton<IMessageBus>(new MessageBus(connection));

        return services;
    }
}