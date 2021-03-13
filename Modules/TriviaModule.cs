using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities.Trivia;
using System;
using System.Collections.Generic;
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
                List<DefaultAnswer> tempAns = new List<DefaultAnswer>() 
                { 
                    new DefaultAnswer("The A answer", false),
                    new DefaultAnswer("The B answer", false),
                    new DefaultAnswer("The C answer", false),
                    new DefaultAnswer("The D answer", true),
                    new DefaultAnswer("The E answer", false),
                };
                IQuestion<DefaultAnswer> question = new DefaultQuestion("This is the Question", tempAns);
                if (question.GetAnswersLenght() > 5) throw new Exception("The Question can only have up to 5 answers because I dind't find a way to generalize the emoji creation");

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = question.GetQuestion()
                };

                string[] ansLetters = new string[] { "A:", "B:", "C:", "D:", "E:" };
                var ans = question.GetAnswers();
                for(int i = 0; i < ans.Count; i++)
                {
                    embed.AddField(ansLetters[i], ans[i].Content, false);
                }

                var templateReactions = new Emoji[] { new Emoji("🇦"), new Emoji("🇧"), new Emoji("🇨"), new Emoji("🇩"), new Emoji("🇪") };
                List<Emoji> reactions = new List<Emoji>(templateReactions[0..question.GetAnswersLenght()]);

                var r = ReplyAsync(embed: embed.Build()).Result;
                await r.AddReactionsAsync(reactions.ToArray()).ConfigureAwait(false);


                async Task MediatorFunc(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
                   => await Task.Run(() => ReactionAdded(message, channel, reaction, reactions[0], r));

                Context.Client.ReactionAdded += MediatorFunc;

                Task task = new Task(async () =>
                {
                    await ReplyAsync("hi");
                });
                task.Wait(TimeSpan.FromSeconds(10));

                Context.Client.ReactionAdded -= MediatorFunc;
            });
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction, Emoji correctReaction, IUserMessage referenceMessage)
        {
            await Task.Run(async () =>
            {
                 if ((message.Value?.Id ?? referenceMessage.Id - 1) != referenceMessage.Id) await Task.CompletedTask;
                 if (reaction.User.Value.IsBot) await Task.CompletedTask;

                 if (reaction.Emote.Name == correctReaction.Name) await ReplyAsync(reaction.User.Value.Username);
                 await Task.CompletedTask;
            });
        }
    }
}
