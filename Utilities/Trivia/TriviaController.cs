using Discord;
using Discord.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Utilities.Trivia
{
    public class TriviaController<T> where T : BaseAnswer
    {
        public ulong channelId;
        public Dictionary<IUser, string> dictionary;
        public IQuestion<T> Question { get; private set; }
        public IUserMessage Message { get; set; }
        List<Emoji> reactions;

        public TriviaController(ulong channelId)
        {
            this.channelId = channelId;
            dictionary = new Dictionary<IUser, string>();
        }

        public TriviaController(IQuestion<T> question)
        {
            Question = question;
            dictionary = new Dictionary<IUser, string>();
        }

        public List<IUser> GetCorrectUsers(string correctAns)
        {
            List<IUser> users = new List<IUser>();
            foreach (var e in dictionary)
            {
                if (e.Value == correctAns)
                {
                    users.Add(e.Key);
                }
            }         

            return users;
        }     

        public void SetMessageAndChannel(IUserMessage message)
        {
            Message = message;
            channelId = message.Channel.Id;
        }

        public async Task HandleReactionsAsync()
        {
            var templateReactions = new Emoji[] { new Emoji("🇦"), new Emoji("🇧"), new Emoji("🇨"), new Emoji("🇩"), new Emoji("🇪") };
            reactions = new List<Emoji>(templateReactions[0..Question.GetAnswersLenght()]);
            await Message.AddReactionsAsync(reactions.ToArray()).ConfigureAwait(false);
        }

        public Task HandleReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            return Task.Run(() =>
            {
                if ((message.Value?.Id ?? Message.Id - 1) != Message.Id) return;
                if (reaction.User.Value.IsBot) return;

                Console.WriteLine("Reaction Added!");

                var user = reaction.User.Value;
                var ans = reaction.Emote.Name;
                if (dictionary.ContainsKey(user))
                {
                    dictionary[user] = ans;
                }
                else dictionary.Add(user, ans);
            });
        }

        public Embed GetQuestionEmbed()
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = Question.GetQuestion()
            };

            string[] ansLetters = new string[] { "A:", "B:", "C:", "D:", "E:" };
            var ans = Question.GetAnswers();
            for (int i = 0; i < ans.Count; i++)
            {
                embed.Description += $"**{ansLetters[i]}** {ans[i].Content}\n";
            }

            return embed.Build();
        }

        public Embed WhenTimeout()
        {
            Console.WriteLine("Time Out!");
            var tmp = GetCorrectUsers(reactions[Question.GetCorrectAnswerIndex()].Name);

            EmbedBuilder embed = new EmbedBuilder() 
            { 
                Title = $"The right answer was {reactions[Question.GetCorrectAnswerIndex()].Name}"
            };
            for (int i = 0; i < tmp.Count; i++)
            {
                embed.Description += $" - **{tmp[i].Username}**\n";
                Console.WriteLine(tmp[i].ToString());
            }
            embed.Description += embed.Description == "" ? "No one got it right" : "got it right!";

            return embed.Build();
        }

        //public IQuestion GetRandomQuestion() { }
    }
}
