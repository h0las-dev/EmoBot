﻿using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EmoBot.Converter.Models.RabbitMQ
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RabbitOptions _options;

        private readonly IConnection _connection;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> options)
        {
            _options = options.Value;
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
            else
            {
                obj?.Dispose();
                return false;
            }
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port,
                VirtualHost = _options.VHost,
            };

            return factory.CreateConnection();
        }
    }
}
