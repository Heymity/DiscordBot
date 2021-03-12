using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Questao?"
                };
                embed.AddField("A:", "A alternativa", true);
                var r = ReplyAsync(embed: embed.Build()).Result;

                var reactions = new List<Emoji>() { new Emoji("🇦"), new Emoji("🇧"), new Emoji("🇨"), new Emoji("🇩"), new Emoji("🇪") };
                await Task.Run(() => r.AddReactionsAsync(reactions.ToArray()).ConfigureAwait(false));

                Context.Client.ReactionAdded += (Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
                    => Task.Run(() => ReactionAdded(message, channel, reaction, reactions[0], r));

                Task task = new Task(async () =>
                {
                    await ReplyAsync("hi");
                });
                task.Wait(TimeSpan.FromSeconds(10));
                
            });
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction, Emoji correctRreaction, IUserMessage referenceMessage)
        {
            await Task.Run(async () =>
            {
                 if ((message.Value?.Id ?? referenceMessage.Id - 1) != referenceMessage.Id) await Task.CompletedTask;
                 if (reaction.User.Value.IsBot) await Task.CompletedTask;

                 if (reaction.Emote.Name == correctRreaction.Name) await ReplyAsync(reaction.User.Value.Username);
                 await Task.CompletedTask;
            });
        }
    }
}
