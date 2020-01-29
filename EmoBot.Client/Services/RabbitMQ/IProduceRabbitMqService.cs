namespace EmoBot.Client.Services.RabbitMQ
{
    public interface IProduceRabbitMqService
    {
        void Send<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class;
    }
}
