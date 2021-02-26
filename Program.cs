using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot
{
	public class Program
	{
		private DiscordSocketClient client;

		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			var config = new DiscordSocketConfig { MessageCacheSize = 100 };
			client = new DiscordSocketClient(config);

			client.Log += Log;

			await client.LoginAsync(TokenType.Bot, File.ReadAllText("C:/Users/GABRIEL/Desktop/Lang Files/C# Files/DiscordBot/DiscordBot/Token.txt"));
			await client.StartAsync();
			
			client.MessageUpdated += MessageUpdated;
            client.MessageReceived += MessageReceived;
			client.Ready += () =>
			{
				Console.WriteLine("Bot is connected!");
				client.SetGameAsync("os testes pela janela");
				return Task.CompletedTask;
			};

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task MessageReceived(SocketMessage msg)
		{
			Console.WriteLine($"({msg.Channel}, {msg.Author}) -> {msg.Content}");
			return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
		{
			// If the message was not in the cache, downloading it will result in getting a copy of `after`.
			var message = await before.GetOrDownloadAsync();
			Console.WriteLine($"({message.Channel}, {message.Author}): {message} -> {after}");
		}
	}
}
