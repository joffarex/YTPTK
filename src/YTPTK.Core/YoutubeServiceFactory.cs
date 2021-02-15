using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace YTPTK.Core
{
    public class YoutubeServiceFactory
    {
        public static YouTubeService Create(string apiKey)
        {
            var initializer = new BaseClientService.Initializer();
            initializer.ApiKey = apiKey;
            var youTubeService = new YouTubeService(initializer);
            return youTubeService;
        }
    }
}