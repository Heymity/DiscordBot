using Discord;
using Discord.Commands;
using DiscordBot.Utilities.Managers.Storage;
using DiscordBot.Utilities.Trivia;
using System;
using System.Threading.Tasks;
using System.Timers;

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

                if (triviaController.Question.GetAnswersLenght() > 5) throw new Exception("The Question can only have up to 5 answers because I dind't find a way to generalize the emoji creation for the reactions yet ;)");

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
            var userData = DataStorageManager.Current[Context.Guild.Id].GuildTriviaData.GetUserData(user.Id);

            EmbedBuilder embed;

            if (userData != null)
            {
                embed = new EmbedBuilder()
                {
                    Title = $"{user.Username} trivia profile",
                    Description = $"{user.Username} has {userData.score} points with a correct answer rate of {((float)userData.answeredCorrectly / (userData.answered == 0 ? 1 : userData.answered)) * 100}%, answering a total of {userData.answered} questions, being {userData.answeredCorrectly} of those correctly answered.",
                };
            }
            else
            {
                embed = new EmbedBuilder()
                {
                    Title = $"The user {user.Username} has not answered a trivia question in this server yet.",
                    Description = "what a shame"
                };
            }

            await ReplyAsync(embed: embed.Build());
        }
    }
}