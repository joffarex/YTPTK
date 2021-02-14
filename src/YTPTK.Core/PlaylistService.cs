using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YtPlaylist = Google.Apis.YouTube.v3.Data.Playlist;

namespace YTPTK.Core
{
    public class PlaylistService
    {
        public PlaylistService(string apiKey)
        {
            var initializer = new BaseClientService.Initializer();
            initializer.ApiKey = apiKey;
            YouTubeService = new YouTubeService(initializer);
        }

        private YouTubeService YouTubeService { get; }

        public static int ParseDurationToSeconds(string playlistDuration)
        {
            var days = 0;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;

            if (Playlist.DurationFormat.IsMatch(playlistDuration))
            {
                MatchCollection matches = Playlist.DurationFormat.Matches(playlistDuration);

                foreach (Group group in matches[0].Groups)
                {
                    switch (group.Name)
                    {
                        case "1":
                            days += !string.IsNullOrEmpty(group.Value) ? int.Parse(group.Value) : 0;
                            break;
                        case "2":
                            hours += !string.IsNullOrEmpty(group.Value) ? int.Parse(group.Value) : 0;
                            break;
                        case "3":
                            minutes += !string.IsNullOrEmpty(group.Value) ? int.Parse(group.Value) : 0;
                            break;
                        case "4":
                            seconds += !string.IsNullOrEmpty(group.Value) ? int.Parse(group.Value) : 0;
                            break;
                    }
                }
            }

            int totalSeconds = days * 86400 + hours * 3600 + minutes * 60 + seconds;
            return totalSeconds;
        }

        public async Task<List<Video>> FetchPlaylistVideos
            (string playlistId, int videosPerPage, string pageToken = null)
        {
            PlaylistItemListResponse playlistResponse = await FetchPlaylistPage(playlistId, videosPerPage, pageToken);
            IEnumerable<string> videoIds = GetPlaylistPageVideoIds(playlistResponse.Items);

            VideoListResponse videoResponse = await FetchVideos(videoIds);

            List<Video> videos = videoResponse.Items.Select(video => new Video
            {
                Id = video.Id, Title = video.Snippet.Title,
                Duration = ParseDurationToSeconds(video.ContentDetails.Duration),
            }).ToList();

            if (!string.IsNullOrEmpty(playlistResponse.NextPageToken))
            {
                List<Video> response =
                    await FetchPlaylistVideos(playlistId, videosPerPage, playlistResponse.NextPageToken);
                videos.AddRange(response);
            }

            return videos;
        }

        public async Task<Playlist> GetPlaylist(string playlistId)
        {
            var playlist = new Playlist(playlistId);
            YtPlaylist info = await FetchPlaylistInfo(playlist.Id);
            List<Video> videos = await FetchPlaylistVideos(playlist.Id, 50);
            playlist.Title = info.Snippet.Title;
            playlist.Videos = videos;

            return playlist;
        }

        public async Task<int> FetchPlaylistDuration(string playlistId, int videosPerPage, string pageToken = null)
        {
            PlaylistItemListResponse playlistResponse = await FetchPlaylistPage(playlistId, videosPerPage, pageToken);
            IEnumerable<string> videoIds = GetPlaylistPageVideoIds(playlistResponse.Items);

            VideoListResponse videoResponse = await FetchVideos(videoIds);

            int duration = videoResponse.Items.Sum(videoResponseItem =>
                ParseDurationToSeconds(videoResponseItem.ContentDetails.Duration));

            if (!string.IsNullOrEmpty(playlistResponse.NextPageToken))
            {
                duration += await FetchPlaylistDuration(playlistId, videosPerPage, playlistResponse.NextPageToken);
            }

            return duration;
        }

        public async Task<YtPlaylist> FetchPlaylistInfo(string playlistId)
        {
            PlaylistsResource.ListRequest request = YouTubeService.Playlists.List("contentDetails,id,status,snippet");
            request.Id = playlistId;
            PlaylistListResponse response = await request.ExecuteAsync();

            return response.Items[0];
        }

        public async Task<PlaylistItemListResponse> FetchPlaylistPage
            (string playlistId, int videosPerPage, string pageToken = null)
        {
            PlaylistItemsResource.ListRequest request =
                YouTubeService.PlaylistItems.List("contentDetails,id,status,snippet");
            request.PlaylistId = playlistId;
            request.MaxResults = videosPerPage;
            request.PageToken = pageToken;
            PlaylistItemListResponse response = await request.ExecuteAsync();
            return response;
        }

        public IEnumerable<string> GetPlaylistPageVideoIds(IEnumerable<PlaylistItem> playlistItems)
        {
            return playlistItems.Select(playlistItem => playlistItem.ContentDetails.VideoId).ToList();
        }

        public async Task<VideoListResponse> FetchVideos(IEnumerable<string> videoIds)
        {
            VideosResource.ListRequest request = YouTubeService.Videos.List("contentDetails,id,status,snippet");
            request.Id = string.Join(",", videoIds);
            VideoListResponse response = await request.ExecuteAsync();
            return response;
        }
    }
}