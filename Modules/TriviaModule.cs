using Discord.Commands;
using DiscordBot.Utilities.Trivia;
using System;
using System.Timers;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Utilities.Managers.Storage;

namespace DiscordBot.Modules
{
    [Group("trivia")]
    public class TriviaModule : ModuleBase<SocketCommandContext>
    {
        [Command] 
        public async Task Trivia([Remainder][Summary("The theme")] string theme = "")
        {
            await Task.Run(async () =>
            {
                TriviaController<BaseAnswer> triviaController = new TriviaController<BaseAnswer>(Context.Guild);
                triviaController.GetRandomQuestion();

                if (triviaController.Question.GetAnswersLenght() > 5) throw new Exception("The Question can only have up to 5 answers because I dind't find a way to generalize the emoji creation ;)");

                var r = base.ReplyAsync(embed: triviaController.GetQuestionEmbed()).Result;

                triviaController.SetMessage(r);
                await triviaController.HandleReactionsAsync();                              
                Context.Client.ReactionAdded += triviaController.HandleReactionAdded;

                Timer t = new Timer
                {
                    Interval = 10000, 
                    AutoReset = false 
                };

                t.Elapsed += new ElapsedEventHandler(async (object sender, ElapsedEventArgs e) => 
                {
                    Context.Client.ReactionAdded -= triviaController.HandleReactionAdded;
                    t.Dispose();
                    await ReplyAsync(embed: triviaController.WhenTimeout());
                });
                t.Start();
            });
        }  

        [Command("get user")]
        [Alias("gu", "user")]
        public async Task GetUserInfo(IUser user) 
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{user.Username} trivia profile",
                Description = $"{DataStorageManager.Current[Context.Guild.Id].GuildTriviaData.GetUserScore(user.Id)} points"
            };
            await ReplyAsync(embed: embed.Build());
        }
    }
}
