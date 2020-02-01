using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace EmoBot.Client.Services.RabbitMQ
{
    public class ProduceRabbitMqService : IProduceRabbitMqService
    {
        private readonly DefaultObjectPool<IModel> _objectPool;
        private readonly ILogger _logger;

        public ProduceRabbitMqService(IPooledObjectPolicy<IModel> objectPolicy, ILogger<ProduceRabbitMqService> logger)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
            _logger = logger;
        }

        public void Send<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class
        {
            if (message == null)
            {
                return;
            }

            var channel = _objectPool.Get();

            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true, false);

                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Can not send a message to converter service: {ex.Message}");
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }
    }
}
