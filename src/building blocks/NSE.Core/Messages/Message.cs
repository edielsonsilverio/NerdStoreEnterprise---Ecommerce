namespace NSE.Core.Messages;

public abstract class Message
{
    public string MessageType { get; protected set; }
    public Guid AggregateId { get; protected set; }

    protected Message()
    {
        //Pegar o tipo de mensagem (comando, handler,...)
        MessageType = GetType().Name;
    }
}