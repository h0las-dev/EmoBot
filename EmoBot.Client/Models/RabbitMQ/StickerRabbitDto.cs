namespace EmoBot.Client.Models.RabbitMQ
{
    public class StickerRabbitDto
    {
        public string StickerId { get; set; }
        public string StickerUniqueId { get; set; }
        public string GiphyUrl { get; set; }
        public long TelegramChatId { get; set; }
        public int TelegramMessageId { get; set; }
    }
}
