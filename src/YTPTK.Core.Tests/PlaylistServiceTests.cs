using Google.Apis.YouTube.v3;
using Moq;
using Xunit;
using YTPTK.Core.Tests.Data;

namespace YTPTK.Core.Tests
{
    public class PlaylistServiceTests
    {
        private readonly PlaylistService _playlistService;

        public PlaylistServiceTests()
        {
            Mock<YouTubeService> youtubeServiceMock = new();
            _playlistService = new PlaylistService(youtubeServiceMock.Object);
        }

        [Theory]
        [ClassData(typeof(DurationToSecondsData))]
        public void ParseDurationToSecondsTheory(int expected, string input)
        {
            int seconds = _playlistService.ParseDurationToSeconds(input);

            Assert.Equal(expected, seconds);
        }
    }
}