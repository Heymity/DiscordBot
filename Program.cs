#define DEBUG
//#undef DEBUG

using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using DiscordBot.Logging;
using DiscordBot.Commands;
using Discord.Commands;

namespace DiscordBot
{
	public class Program
	{
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

			await client.LoginAsync(TokenType.Bot, File.ReadAllText("C:/Users/GABRIEL/Desktop/Lang Files/C# Files/DiscordBot/DiscordBot/Token.txt"));
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
			
			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

#if DEBUG
		private Task MessageReceived(SocketMessage msg)
		{
			Console.WriteLine($"({msg.Channel}, {msg.Author}) -> {msg.Content}");
			return Task.CompletedTask;
        }

		private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
		{
			// If the message was not in the cache, downloading it will result in getting a copy of `after`.
			var message = await before.GetOrDownloadAsync();
			Console.WriteLine($"({message.Channel}, {message.Author}): {message} -> {after}");
		}
#endif
	}
}
