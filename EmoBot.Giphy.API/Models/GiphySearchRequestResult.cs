using System.Collections.Generic;

namespace EmoBot.Giphy.API.Models
{
    public class GiphySearchRequestResult
    {
        public IEnumerable<Gif> Data { get; set; }
    }
}
