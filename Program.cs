#define DEBUG
//#undef DEBUG

using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Logging;
using DiscordBot.Commands;
using DiscordBot.Utilities.Managers.Storage;
using DiscordBot.Utilities.Trivia;

namespace DiscordBot
{
	public class Program
	{
#if DEFAULTDIR
		private const string TOKEN_DIRECTORY = "C:/Users/GABRIEL/Desktop/Lang Files/C# Files/DiscordBot/DiscordBot/Token.txt";
#else
		private const string TOKEN_DIRECTORY = "";
#endif
		private DiscordSocketClient client;
		private LoggingService loggingService;
		private CommandHandler commandHandler;
		private CommandService commandService;

		private DataStorageManager dataStorageManager;

		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			client = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 100 });
			commandService = new CommandService();
			loggingService = new LoggingService(client, commandService);
			commandHandler = new CommandHandler(client, commandService);

			await commandHandler.InstallCommandsAsync();

			await client.LoginAsync(TokenType.Bot, File.ReadAllText(TOKEN_DIRECTORY));
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

			dataStorageManager = new DataStorageManager()
			{
				GeneralTriviaData = new TriviaData<BaseAnswer>()
                {
					questions = new System.Collections.Generic.List<IQuestion<BaseAnswer>>()
                    {
						new BaseQuestion("This is a question", new System.Collections.Generic.List<BaseAnswer>() 
						{ 
							new BaseAnswer("This is the correct alternative A", true),
							new BaseAnswer("This is the alternative B", false),
							new BaseAnswer("This is the alternative C", false),
							new BaseAnswer("This is the alternative D", false),
							new BaseAnswer("This is the alternative E", false),
						}, 1),
						new BaseQuestion("This is a question", new System.Collections.Generic.List<BaseAnswer>()
						{
							new BaseAnswer("This is the alternative A", false),
							new BaseAnswer("This is the correct alternative B", true),
							new BaseAnswer("This is the alternative C", false),
							new BaseAnswer("This is the alternative D", false),
							new BaseAnswer("This is the alternative E", false),
						}, 1),
						new BaseQuestion("This is a question", new System.Collections.Generic.List<BaseAnswer>()
						{
							new BaseAnswer("This is the alternative A", false),
							new BaseAnswer("This is the alternative B", false),
							new BaseAnswer("This is the correctalt ernative C", true),
							new BaseAnswer("This is the alternative D", false),
							new BaseAnswer("This is the alternative E", false),
						}, 1),
						new BaseQuestion("This is a question", new System.Collections.Generic.List<BaseAnswer>()
						{
							new BaseAnswer("This is the alternative A", false),
							new BaseAnswer("This is the alternative B", false),
							new BaseAnswer("This is the alternative C", false),
							new BaseAnswer("This is the correct alternative D", true),
							new BaseAnswer("This is the alternative E", false),
						}, 1),
						new BaseQuestion("This is a question", new System.Collections.Generic.List<BaseAnswer>()
						{
							new BaseAnswer("This is the alternative A", false),
							new BaseAnswer("This is the alternative B", false),
							new BaseAnswer("This is the alternative C", false),
							new BaseAnswer("This is the alternative D", false),
							new BaseAnswer("This is the correct alternative E", true),
						}, 1)
					},
					usersScores = new System.Collections.Generic.Dictionary<ulong, int>()
                },
			};
			DataStorageManager.Current.SaveData();
			//DataStorageManager.Current.LoadData();

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
