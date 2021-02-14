using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using YTPTK.Core;

namespace YTPTK.Cli
{
    internal class Program
    {
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
                Console.WriteLine("  ytp init <GOOGLE_API_KEY>");
                Console.WriteLine("  ytp <PLAYLIST_LINK|PLAYLIST_ID>");
            }

            if (args.Length == 2 && args[0] == "init")
            {
                var credentials = new Credentials {ApiKey = args[1]};
                using FileStream stream = File.Create("credentials.json");
                await JsonSerializer.SerializeAsync(stream, credentials);

                Console.WriteLine("API_KEY saved...");
            }

            if (args.Length == 1)
            {
                Console.WriteLine("Processing...");

                try
                {
                    using FileStream stream = File.OpenRead("credentials.json");
                    var credentials = await JsonSerializer.DeserializeAsync<Credentials>(stream);

                    var playlistService = new PlaylistService(credentials.ApiKey);

                    Playlist playlist = await playlistService.GetPlaylist(Playlist.TryParseUrl(args[0]));
                    Console.WriteLine(playlist);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}