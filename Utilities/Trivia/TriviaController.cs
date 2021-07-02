using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SetMessage(IUserMessage message) => Message = message;

        public List<IUser> ForEachUser(Func<IUser, string, bool> action)
        {
            List<IUser> users = new List<IUser>();
            foreach(var userAns in UsersAnswers)
            {
                if (action.Invoke(userAns.Key, userAns.Value))
                    users.Add(userAns.Key);
            }
            return users;
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
                Title = Question.GetQuestion(),
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Value: {Question.Points} {(Question.Points == 1 ? "point" : "points")}"
                }
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

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"The right answer was {reactions[Question.GetCorrectAnswerIndex()].Name}"
            };

            var correctUsers = ForEachUser((user, ans) => 
            {
                if (ans == reactions[Question.GetCorrectAnswerIndex()].Name)
                {
                    var score = DataStorageManager.Current[Guild.Id].GuildTriviaData.QuestionSolved(user, true, Question.Points).score;
                    embed.Description += $" - **{user.Username}** ({score} points)\n";
                    return true;
                }
                else
                {
                    var score = DataStorageManager.Current[Guild.Id].GuildTriviaData.QuestionSolved(user, false, Question.Points).score;
                }

                return false;
            });
            embed.Description += correctUsers.Any() ? "got it right!" : "No one got it right";

            AutoSaveManager.ReduceIntervalByChangePriority(ChangePriority.UserDataChange);

            return embed.Build();
        }

        public IQuestion<T> GetRandomQuestion() => Question = (IQuestion<T>)DataStorageManager.Current.GetRandomQuestion(Guild.Id);
    }
}