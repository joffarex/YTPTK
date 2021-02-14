using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace YTPTK.Core
{
    public class Playlist
    {
        private const string LinkPrefix = "https://www.youtube.com/playlist?list=";
        private List<Video> _videos;

        public Playlist(string id)
        {
            Id = TryParseUrl(id);
            Videos = new List<Video>();
            Duration = 0;
        }

        public Playlist(string id, string title, List<Video> videos)
        {
            Id = TryParseUrl(id);
            Title = title;
            Videos = videos;
            CalculateFullDuration();
        }

        public string Id { get; }
        public string Title { get; set; }
        public int Duration { get; private set; }
        public static Regex DurationFormat { get; } = new(@"^P(?:(\d+)D)?T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?$");

        public string FormattedDuration
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(Duration);
                return time.Days != 0 ? time.ToString(@"dd\.hh\:mm\:ss") : time.ToString(@"hh\:mm\:ss");
            }
        }

        public List<Video> Videos
        {
            get => _videos;
            set
            {
                _videos = value;
                CalculateFullDuration();
            }
        }

        public static string TryParseUrl(string id) => id.Contains(LinkPrefix) ? id.Replace(LinkPrefix, "") : id;

        public void CalculateFullDuration()
        {
            Duration = Videos.Sum(video => video.Duration);
        }

        public override string ToString() =>
            $"ID: {Id}\nLink: {LinkPrefix}{Id}\nTitle: {Title}\nDuration: {FormattedDuration}";
    }
}