using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmoBot.Converter.Models;
using EmoBot.Converter.Models.RabbitMQ;
using EmoBot.Giphy.API.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmoBot.Converter.Services.RabbitMQ
{
    public class ConsumeRabbitMqService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger _logger;
        private readonly GiphyOptions _giphyOptions;
        private readonly IGiphyService _giphyService;
        private readonly IProduceRabbitMqService _rabbitSendingService;

        public ConsumeRabbitMqService(ILogger<ConsumeRabbitMqService> logger,
            IOptions<GiphyOptions> giphyOptions, IGiphyService giphyService, IProduceRabbitMqService rabbitSendingService)
        {
            _logger = logger;
            _giphyOptions = giphyOptions.Value;

            _giphyService = giphyService;
            _rabbitSendingService = rabbitSendingService;

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

            _channel.BasicConsume("telegram.bot.converter", false, consumer);
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
            _logger.LogInformation($"converter received {message}");

            var emojiFromClient = JsonConvert.DeserializeObject<EmojiRabbitDto>(message);

            var giphyData = await _giphyService.GetGifBasedOnSearchEmoji(emojiFromClient.EmojiValue, 
                _giphyOptions.Token, _giphyOptions.SearchEndpointUrl);

            var gif = giphyData.Data.FirstOrDefault();
            if (gif != null)
            {
                emojiFromClient.GiphyUrl = gif.Url;
            }
            
            _rabbitSendingService.Send(
                emojiFromClient,
                "telegram.bot",
                "direct",
                "client.endpoint");
        }

        private void InitRabbitMq()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("telegram.bot", ExchangeType.Direct, durable: true);
            _channel.QueueDeclare("telegram.bot.converter", true, false, false);
            _channel.QueueBind("telegram.bot.converter", "telegram.bot", "converter.endpoint");
            _channel.BasicQos(0, 1, false);
        }
    }
}
