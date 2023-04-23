namespace IP2C_Web_API.Interfaces;

public interface IMessageProducer
{
    public void SendingMessage<T>(T message);
}
