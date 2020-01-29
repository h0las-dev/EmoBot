using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EmoBot.Client.Models.RabbitMQ
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RabbitOptions _options;
        private readonly IConnection _connection;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> rabbitOptions)
        {
            _options = rabbitOptions.Value;
            _connection = GetConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            
            obj.Dispose();
            return false;
        }

        private IConnection GetConnection()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port,
                VirtualHost = _options.VHost,
            };

            return connectionFactory.CreateConnection();
        }
    }
}
