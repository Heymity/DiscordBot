#define DEBUG
//#undef DEBUG
#undef DEFAULTDIR
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Logging;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//using DiscordBot.Utilities.Trivia;

namespace DiscordBot
{
    public class Program
    {
#if DEFAULTDIR
        public const string DIRECTORY = "C:/Users/GABRIEL/Desktop/LangFiles/C#/DiscordBot/DiscordBot";
#else
		public const string DIRECTORY = "D:/Gabriel/LangFiles/DiscordBot/DiscordBot";
#endif
        private readonly string tokenDir = $"{DIRECTORY}/Token.txt";

        private DiscordSocketClient client;
        private LoggingService loggingService;
        private CommandHandler commandHandler;
        private CommandService commandService;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            await Login();

            client.Ready += WhenReady;

            DataStorageManager.Current.LoadData();
            DataStorageManager.Current.LoadNewQuestionsFromJson();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Login()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 100 });
            commandService = new CommandService();
            loggingService = new LoggingService(client, commandService);
            commandHandler = new CommandHandler(client, commandService);

            await commandHandler.InstallCommandsAsync();

            await client.LoginAsync(TokenType.Bot, File.ReadAllText(tokenDir));
            await client.StartAsync();

#if DEBUG
            client.MessageUpdated += MessageUpdated;
            client.MessageReceived += MessageReceived;
#endif
        }

        private async Task WhenReady()
        {
            Console.WriteLine("Bot is connected!");

            IVoiceChannel channel;
            while (true)
            {
                try
                {
                    channel = client.Guilds.First().VoiceChannels.First();
                    break;
                }
                catch (InvalidOperationException) { }
            }
            IAudioClient audioClient = await channel.ConnectAsync();
            AudioOutStream stream = audioClient.CreateOpusStream();

            stream.Write(new byte[1000], 0, 1000);

            await client.SetGameAsync("o henrique pela janela");
        }

#if DEBUG

        private async Task MessageReceived(SocketMessage msg)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"({msg.Channel}, {msg.Author}) -> {msg.Content}");
                return Task.CompletedTask;
            });
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            await Task.Run(async () =>
            {
                var message = await before.GetOrDownloadAsync();
                Console.WriteLine($"({message.Channel}, {message.Author}): {message} -> {after}");
            });
        }

#endif
    }
}