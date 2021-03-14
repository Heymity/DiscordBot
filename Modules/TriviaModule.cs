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

                var r = base.ReplyAsync(embed: embed.Build()).Result;
                await r.AddReactionsAsync(reactions.ToArray()).ConfigureAwait(false);

                var correctIndex = question.GetCorrectAnswerIndex();
                async Task Handler(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction) => await Task.Run(() => ReactionAdded(message, channel, reaction, reactions[correctIndex], r));

                Context.Client.ReactionAdded += Handler;

                Timer t = new Timer
                {
                    Interval = 10000, 
                    AutoReset = false 
                };

                t.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => 
                {
                    WhenTimeout(sender, e);
                    Context.Client.ReactionAdded -= Handler;
                    t.Dispose();
                });

                t.Start();

            });
        }

        void WhenTimeout(object sender, ElapsedEventArgs e)
        {
            ReplyAsync("Time Out!");
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction, Emoji correctReaction, IUserMessage referenceMessage)
        {
            await Task.Run(async () =>
            {
                if ((message.Value?.Id ?? referenceMessage.Id - 1) != referenceMessage.Id) return;
                if (reaction.User.Value.IsBot) return;

                if (reaction.Emote.Name == correctReaction.Name) await ReplyAsync(reaction.User.Value.Username);
                return;
            });
        }
    }
}
