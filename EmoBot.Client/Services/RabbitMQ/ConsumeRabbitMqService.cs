using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmoBot.Client.Helpers;
using EmoBot.Client.Models;
using EmoBot.Client.Models.RabbitMQ;
using EmoBot.Client.Services.Converter;
using EmoBot.Client.Services.Data;
using EmoBot.Giphy.API.Services;
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
        private readonly IConverterService _converterService;
        private readonly IGiphyService _giphyService;

        public ConsumeRabbitMqService(ILogger<ConsumeRabbitMqService> logger, CacheService cacheService, 
            ITelegramBotService telegramBotService, IConverterService converterService, IGiphyService giphyService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _telegramBotService = telegramBotService;
            _converterService = converterService;
            _giphyService = giphyService;

            InitRabbitMq();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(ea.Body);

                try
                {
                    await HandleMessage(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);
                }
            };

            _channel.BasicConsume("converter.queue", false, consumer);
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

            var stickerFromClient = JsonConvert.DeserializeObject<StickerRabbitDto>(message);
            var chatId = stickerFromClient.TelegramChatId;
            var messageId = stickerFromClient.TelegramMessageId;
            var stickerId = stickerFromClient.StickerId;
            var uniqueStickerId = stickerFromClient.StickerUniqueId;

            // Качаем стикер с телеграма
            var fileInfo = await _telegramBotService.DownloadFileAsync(stickerId);

            // Конвертируем из .tgs или .webp в .gif
            var convertedFileInfo = await _converterService.ConvertAsync(fileInfo);
            var fileData = await File.ReadAllBytesAsync(Constants.ConverterPath + convertedFileInfo.Name);

            // Загружаем на Giphy
            var uploadResult = await _giphyService.UploadFileAsync(fileData);

            // Получаем информацию о загруженной Gif
            var getResult = await _giphyService.GetGifAsync(uploadResult.Data.Id);

            // Добавляем запись в кэш
            _cacheService.Create(new Sticker
            {
                GiphyUrl = getResult.Data.Url,
                UniqueStickerId = uniqueStickerId
            });

            // Отправляем пользователю ссылку
            await _telegramBotService.SendMessageAsync(chatId, messageId, getResult.Data.Url);

            // Удаляем скачанный и сконвертированный файлы
            DeleteTmpFiles();
        }

        private void InitRabbitMq()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("telegram.bot", ExchangeType.Direct, true, false);
            _channel.QueueDeclare("converter.queue", true, false, false);
            _channel.QueueBind("converter.queue", "telegram.bot", "converter.endpoint");
            _channel.BasicQos(0, 1, false);
        }

        private void DeleteTmpFiles()
        {
            var png = Directory.GetFiles(Constants.ConverterPath, "*.png").AsEnumerable();
            var gif = Directory.GetFiles(Constants.ConverterPath, "*.gif").AsEnumerable();
            var webp = Directory.GetFiles(Constants.ConverterPath, "*.webp").AsEnumerable();
            var tgs = Directory.GetFiles(Constants.ConverterPath, "*.tgs").AsEnumerable();

            var files = new List<string>();
            files.AddRange(png);
            files.AddRange(gif);
            files.AddRange(webp);
            files.AddRange(tgs);

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
