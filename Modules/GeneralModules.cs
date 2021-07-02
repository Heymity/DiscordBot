using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [Alias("gq", "frase aleatoria", "fa", "random quote", "rq")]
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
        [Summary("Saves the current bot data, only the bot admin can issue it")]
        public async Task Save()
        {
            if (Context.Message.Author.Id != 226473285213224963) return;

            Console.WriteLine(Context.Message.Author.Id);
            await ReplyAsync("Saving...");
            DataStorageManager.Current.SaveData();
        }

        [Command("load")]
        [Summary("Loads the bot data from its save, only the bot admin can issue it")]
        public async Task Load()
        {
            if (Context.Message.Author.Id != 226473285213224963) return;

            Console.WriteLine(Context.Message.Author.Id);
            await ReplyAsync("Loading...");
            DataStorageManager.Current.LoadData();
        }

        [Command("change prefix")]
        [Alias("cp")]
        [Summary("Changes the bot command prefix for this server")]
        public Task ChangePrefix([Summary("New Prefix")] char prefix)
        {
            var id = Context.Guild.Id;
            DataStorageManager.Current[id].CommandPrefix = prefix;
            AutoSaveManager.ReduceIntervalByChangePriority(ChangePriority.GuildDataChange);
            return ReplyAsync($"The command prefix for this server is now {DataStorageManager.Current[id].CommandPrefix}");
        }

        ///TODO: Add the params to the help info.
        [Command("help")]
        [Summary("the help command. I think is very self explanatory.")]
        public async Task HelpCommand()
        {
            var embed = new EmbedBuilder()
            {
                Title = "This is the list of all command for this bot",
                Color = new Color(10, 180, 10)
            };

            var modules = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ModuleBase<SocketCommandContext>))).ToList();

            modules.ForEach(t =>
            {
                embed.Description += $"\n**{t.Name.Replace("Module", null).Replace("Modules", null)} Methods**";
                var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();

                var group = t.GetCustomAttribute<GroupAttribute>();

                methods.ForEach(mi =>
                {
                    var summary = mi.GetCustomAttribute<SummaryAttribute>();
                    var command = mi.GetCustomAttribute<CommandAttribute>();
                    var aliases = mi.GetCustomAttribute<AliasAttribute>();
                    var groupName = "";

                    if (group != null) groupName = group.Prefix + " ";

                    if (command == null) return;

                    embed.Description += $"\n**{DataStorageManager.Current[Context.Guild.Id].CommandPrefix}{groupName}{command.Text}**";

                    if (aliases != null)
                        Array.ForEach(aliases.Aliases, action: a => embed.Description += $" or **{DataStorageManager.Current[Context.Guild.Id].CommandPrefix}{groupName}{(a == "**" ? "\\*\\*" : a)}**");

                    if (summary != null)
                        embed.Description += $"\n{summary.Text}";

                    embed.Description += "\n";
                });
            });

            await ReplyAsync(embed: embed.Build());
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