using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Utilities.Trivia
{
    public class TriviaController<T> where T : BaseAnswer
    {
        public SocketGuild Guild { get; set; }
        public Dictionary<IUser, string> UsersAnswers { get; set; }
        public IQuestion<T> Question { get; private set; }
        public IUserMessage Message { get; private set; }
        private List<Emoji> reactions;

        public TriviaController(SocketGuild guild)
        {
            Guild = guild;
            UsersAnswers = new Dictionary<IUser, string>();
        }

        public TriviaController(IQuestion<T> question, SocketGuild guild)
        {
            Guild = guild;
            Question = question;
            UsersAnswers = new Dictionary<IUser, string>();
        }

        public List<IUser> GetCorrectUsers(string correctAns)
        {
            List<IUser> users = new List<IUser>();
            foreach (var e in UsersAnswers)
            {
                if (e.Value == correctAns)
                {
                    users.Add(e.Key);
                }
            }

            return users;
        }

        public void SetMessage(IUserMessage message) => Message = message;

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
                if (UsersAnswers.ContainsKey(user))
                {
                    UsersAnswers[user] = ans;
                }
                else UsersAnswers.Add(user, ans);
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
            var correctUsers = GetCorrectUsers(reactions[Question.GetCorrectAnswerIndex()].Name);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"The right answer was {reactions[Question.GetCorrectAnswerIndex()].Name}"
            };

            correctUsers.ForEach((IUser user) =>
            {
                var score = DataStorageManager.Current.GetOrCreateGuild(Guild.Id).GuildTriviaData.AddScore(user, Question.Points);

                embed.Description += $" - **{user.Username}** ({score} points)\n";
            });
            embed.Description += correctUsers.Count == 0 ? "No one got it right" : "got it right!";

            AutoSaveManager.ReduceIntervalByChangePriority(ChangePriority.UserDataChange);

            return embed.Build();
        }

        public IQuestion<T> GetRandomQuestion() => Question = (IQuestion<T>)DataStorageManager.Current.GetRandomQuestion(Guild.Id);
    }
}