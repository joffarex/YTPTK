using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using YTPTK.Core;

namespace YTPTK.Cli
{
    internal class Program
    {
        private static PlaylistService PlaylistService { get; set; }

        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                string versionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;

                Console.WriteLine($"ytp v{versionString}");
                Console.WriteLine("-------------");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("  ytp init <GOOGLE_API_KEY>              # GOOGLE_API_KEY: youtube data api key");
                Console.WriteLine(
                    "  ytp <PLAYLIST_LINK|PLAYLIST_ID>        # PLAYLIST_LINK|PLAYLIST_ID: full playlist link or id from url");
                Console.WriteLine(
                    "  ytp file <FILE_PATH>                   # FILE_PATH: path to a file which will contain playlist ids or playlist links on each line, only separated by EOL");
            }

            if (args.Length == 2 && args[0] == "init")
            {
                var credentials = new Credentials {ApiKey = args[1]};
                using FileStream stream = File.Create("credentials.json");
                await JsonSerializer.SerializeAsync(stream, credentials);

                Console.WriteLine("API_KEY saved...");
            }

            if (args.Length == 2 && args[0] == "file")
            {
                Console.WriteLine("Processing...\n");

                try
                {
                    string src = await File.ReadAllTextAsync(args[1]);
                    string[] playlistIds = src.Split(Environment.NewLine);

                    Credentials credentials = await ReadCredentials();
                    YouTubeService youtubeService = YoutubeServiceFactory.Create(credentials.ApiKey);
                    PlaylistService = new PlaylistService(youtubeService);

                    List<Task> logPlaylistTask = playlistIds.Select(playlistId => LogPlaylist(playlistId)).ToList();

                    await Task.WhenAll(logPlaylistTask);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (args.Length == 1)
            {
                Console.WriteLine("Processing...\n");

                try
                {
                    Credentials credentials = await ReadCredentials();
                    YouTubeService youtubeService = YoutubeServiceFactory.Create(credentials.ApiKey);
                    PlaylistService = new PlaylistService(youtubeService);

                    await LogPlaylist(args[0]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static async Task LogPlaylist(string playlistId)
        {
            Playlist playlist = await PlaylistService.GetPlaylist(Playlist.TryParseUrl(playlistId));
            Console.WriteLine(playlist);
            Console.WriteLine();
        }

        private static async Task<Credentials> ReadCredentials()
        {
            using FileStream stream = File.OpenRead("credentials.json");
            var credentials = await JsonSerializer.DeserializeAsync<Credentials>(stream);

            return credentials;
        }
    }
}