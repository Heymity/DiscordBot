using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    public class GeneralModules : ModuleBase<SocketCommandContext>
    {
		// ~say hello world -> hello world
		[Command("say")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);

		// ReplyAsync is a method on ModuleBase 

		[Command("get quote")]
		[Summary("Gets random quote")]
		public async Task GetQuoteAsync([Remainder][Summary("Tags")] string tags = null)
        {
            string quote = await WebRequests.GetAsync("https://api.quotable.io/random");
			Random rnd = new Random();
			var eb = new EmbedBuilder() 
			{ 
				Title = "Random Quote", Description = GetContent(quote), 
				Color = new Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)),
				Author = new EmbedAuthorBuilder() { Name = GetAuthor(quote) }
			};
			await ReplyAsync(embed: eb.Build());
        }

        private string GetContent(string str)
        {
			var tmp = new List<string>(str.Split("\""));
			return tmp[tmp.IndexOf("content") + 2];
        }

		private string GetAuthor(string str)
		{
			var tmp = new List<string>(str.Split("\""));
			return tmp[tmp.IndexOf("author") + 2];
		}
	}
}
