using System.Threading;
using System.Threading.Tasks;
using EmoBot.Client.Models;
using EmoBot.Client.Models.RabbitMQ;
using EmoBot.Client.Services.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmoBot.Client.Services.RabbitMQ
{
    public class ConsumeRabbitMqService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly CacheService _cacheService;
        private readonly ITelegramBotService _telegramBotService;

        public ConsumeRabbitMqService(ILogger<ConsumeRabbitMqService> logger, CacheService cacheService, ITelegramBotService telegramBotService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _telegramBotService = telegramBotService;
            InitRabbitMq();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(ea.Body);
                await HandleMessage(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("telegram.bot.client", false, consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        private async Task HandleMessage(string message)
        {
            _logger.LogInformation($"client received {message}");

            var emojiFromConverter = JsonConvert.DeserializeObject<EmojiRabbitDto>(message);

            var chatId = emojiFromConverter.TelegramChatId;
            var messageId = emojiFromConverter.TelegramMessageId;

            if (emojiFromConverter.GiphyUrl == string.Empty)
            {
                await _telegramBotService.SendMessage(chatId, messageId, 
                    "Кажется, этот emoji не поддерживается ;c");

                _logger.LogWarning($"Giphy did not understand the emoji. ");
            }
            else
            {
                var emojiForUser = _cacheService.Create(new Emoji
                {
                    EmojiValue = emojiFromConverter.EmojiValue,
                    GiphyUrl = emojiFromConverter.GiphyUrl
                });

                await _telegramBotService.SendMessage(chatId, messageId, emojiFromConverter.GiphyUrl);
                _logger.LogWarning($"Client sent emoji link to user: {emojiForUser.GiphyUrl}");
            }
        }

        private void InitRabbitMq()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("telegram.bot", ExchangeType.Direct, true, false);
            _channel.QueueDeclare("telegram.bot.client", true, false, false);
            _channel.QueueBind("telegram.bot.client", "telegram.bot", "client.endpoint");
            _channel.BasicQos(0, 1, false);
        }
    }
}
