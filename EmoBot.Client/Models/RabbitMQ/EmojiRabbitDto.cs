namespace EmoBot.Client.Models.RabbitMQ
{
    public class EmojiRabbitDto
    {
        public string EmojiValue { get; set; }
        public string GiphyUrl { get; set; }
        public long TelegramChatId { get; set; }
        public int TelegramMessageId { get; set; }
    }
}
