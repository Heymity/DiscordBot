using Discord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Trivia
{
    public class TriviaController
    {
        public ulong channelId;
        public Dictionary<IUser, string> dictionary;

        public TriviaController(ulong channelId)
        {
            this.channelId = channelId;
            dictionary = new Dictionary<IUser, string>();
        }

        public void AnswerAdded(IUser user, string ans)
        {
            if(dictionary.ContainsKey(user))
            {
                dictionary[user] = ans;
            }
            else dictionary.Add(user, ans);
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
    }
}
