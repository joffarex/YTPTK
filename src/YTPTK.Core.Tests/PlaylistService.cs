using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace YTPTK.Core.Tests
{
    public class PlaylistService_ParseDurationToSeconds
    {
        [Theory]
        [InlineData(69, "PT69S")]
        [InlineData(60 * 6, "PT6M")]
        [InlineData(60 * 6 + 9, "PT6M9S")]
        [InlineData(60 * 60, "PT1H")]
        [InlineData(60 * 60 + 60 * 6, "PT1H6M")]
        [InlineData(60 * 60 + 60 * 6 + 9, "PT1H6M9S")]
        [InlineData(60 * 60 * 24, "P1DT")]
        [InlineData(60 * 60 * 24 + 60 * 60, "P1DT1H")]
        [InlineData(60 * 60 * 24 + 60 * 60 + 60 * 6, "P1DT1H6M")]
        [InlineData(60 * 60 * 24 + 60 * 60 + 60 * 6 + 9, "P1DT1H6M9S")]
        public void ParseDurationToSeconds(int expected, string input)
        {
            var youtubeServiceMock = new Mock<YouTubeService>();
            var playlistService = new PlaylistService(youtubeServiceMock.Object);
            var seconds = playlistService.ParseDurationToSeconds(input);

            Assert.Equal(expected, seconds);
        }
    }
}