#define DEBUG
//#undef DEBUG
#undef DEFAULTDIR
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Logging;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.IO;
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
            client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                client.SetGameAsync("o henrique pela janela");
                return Task.CompletedTask;
            };

            DataStorageManager.Current.LoadData();
            DataStorageManager.Current.LoadNewQuestionsFromJson();

            // Block this task until the program is closed.
            await Task.Delay(-1);
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