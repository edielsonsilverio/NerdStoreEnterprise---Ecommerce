using EasyNetQ;
using EasyNetQ.Internals;
using NSE.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace NSE.MessageBus;

public class MessageBus : IMessageBus
{
    private IBus _bus;
    private IAdvancedBus _advancedBus;

    private readonly string _connectionString;

    //Receber a string de conexão do RabbitMq pelo construtor.
    public MessageBus(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool IsConnected => _advancedBus?.IsConnected ?? false;
    public IAdvancedBus AdvancedBus => _bus?.Advanced;

    public void Publish<T>(T message) where T : IntegrationEvent
    {
        TryConnect();
        _bus.PubSub.Publish(message);
    }

    public async Task PublishAsync<T>(T message) where T : IntegrationEvent
    {
        TryConnect();
        await _bus.PubSub.PublishAsync(message);
    }

    public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
    {
        TryConnect();
        _bus.PubSub.Subscribe(subscriptionId, onMessage);
    }

    public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
    {
        TryConnect();
        _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
    }

    public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IntegrationEvent
        where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.Request<TRequest, TResponse>(request);
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return await _bus.Rpc.RequestAsync<TRequest, TResponse>(request);
    }

    public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.Respond(responder);
    }

    public AwaitableDisposable<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.RespondAsync(responder);
    }

    //Método responsável por verificar se o serviço está conectado
    private void TryConnect()
    {
        if (IsConnected) return;

        //Caso ocorra algum erro, configura a policy para fazer novas tentativas de conexão.
        var policy = Policy.Handle<EasyNetQException>() // Exception do EasyNetQ
            .Or<BrokerUnreachableException>() // Caso ocorra uma exceção do RabbitMq

            //Vai tentar Três vezes, e cada vez será a cada 1 segundo elevado a 2.
            .WaitAndRetry(3, retryAttempt =>
                 TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        policy.Execute(() =>
        {
            //Cria o Bus [Evento de Mensagem]
            _bus = RabbitHutch.CreateBus(_connectionString);
            _advancedBus = _bus.Advanced;
            _advancedBus.Disconnected += OnDisconnect;
        });
    }

    //Método responsável por desconectar do servidor.
    private void OnDisconnect(object s, EventArgs e)
    {
        var policy = Policy.Handle<EasyNetQException>()
            .Or<BrokerUnreachableException>()
            .RetryForever();

        policy.Execute(TryConnect);
    }

    public void Dispose()
    {
        _bus.Dispose();
    }
}
 