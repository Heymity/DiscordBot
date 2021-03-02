﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    public class GeneralModules : ModuleBase<SocketCommandContext>
    {
		public int counter = 0;

		[Command("say")]
		[Alias("echo")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);

		[Command("get quote")]
		[Alias("gq", "frase aleatoria", "fr", "random quote", "rq")]
		[Summary("Gets random quote")]
		public async Task GetQuoteAsync([Summary("Tags")] params string[] tags)
        {
			string tagsQuery = "";
			if (tags.Length > 0)
            {
				tagsQuery += "?";
				for(int i = 0; i < tags.Length; i++)
                {
					tagsQuery += tags[i];
					if (i != tags.Length - 1) tagsQuery += ",";
                }
            }

            string quote = await WebRequests.GetAsync($"https://api.quotable.io/random{tagsQuery}");

			var content = GetContent(quote);
			var eb = new EmbedBuilder() 
			{ 
				Title = "Random Quote", Description = content, 
				Color = GetColorFromSting(content),
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

		private Color GetColorFromSting(string str)
        {
			int dividerIndex = (int)Math.Floor(str.Length / 3d);

			int r = Math.Abs(str.Substring(0, dividerIndex).GetHashCode() % 255);
			int g = Math.Abs(str.Substring(dividerIndex, 2 * dividerIndex).GetHashCode() % 255);
			int b = Math.Abs(str.Remove(0, 2 * dividerIndex).GetHashCode() % 255);

			return new Color(r, g, b);
        }
	}
}
