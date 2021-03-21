using Discord;
using System.Collections.Generic;

namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public struct TriviaData<T> where T : BaseAnswer
    {
        public List<IQuestion<T>> questions;
        public Dictionary<IUser, int> usersScores;
    }
}
