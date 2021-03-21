using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities.Trivia;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;

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
                List<BaseAnswer> tempAns = new List<BaseAnswer>() 
                { 
                    new BaseAnswer("The A answer", false),
                    new BaseAnswer("The B answer", false),
                    new BaseAnswer("The C answer", false),
                    new BaseAnswer("The D answer", true),
                    new BaseAnswer("The E answer", false),
                };
                IQuestion<BaseAnswer> question = new BaseQuestion("This is the Question", tempAns);
                if (question.GetAnswersLenght() > 5) throw new Exception("The Question can only have up to 5 answers because I dind't find a way to generalize the emoji creation ;)");

                TriviaController<BaseAnswer> triviaController = new TriviaController<BaseAnswer>(question, Context.Guild);

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
    }
}
