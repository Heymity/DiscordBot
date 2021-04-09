using Discord;
using Discord.Commands;
using DiscordBot.Utilities;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class GeneralModules : ModuleBase<SocketCommandContext>
    {
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
                for (int i = 0; i < tags.Length; i++)
                {
                    tagsQuery += tags[i];
                    if (i != tags.Length - 1) tagsQuery += ",";
                }
            }

            string quote = await WebRequests.GetAsync($"https://api.quotable.io/random{tagsQuery}");

            var content = GetContent(quote);
            var eb = new EmbedBuilder()
            {
                Title = "Random Quote",
                Description = content,
                Color = GetColorFromSting(content),
                Author = new EmbedAuthorBuilder() { Name = GetAuthor(quote) }
            };
            await ReplyAsync(embed: eb.Build());
        }

        [Command("save")]
        public async Task Save()
        {
            if (Context.Message.Author.Id != 226473285213224963) return;

            Console.WriteLine(Context.Message.Author.Id);
            await ReplyAsync("Saving...");
            DataStorageManager.Current.SaveData();
        }

        [Command("load")]
        public async Task Load()
        {
            if (Context.Message.Author.Id != 226473285213224963) return;

            Console.WriteLine(Context.Message.Author.Id);
            await ReplyAsync("Loading...");
            DataStorageManager.Current.LoadData();
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