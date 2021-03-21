using Discord;
using System.Collections.Generic;

namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public struct TriviaData<T> where T : BaseAnswer
    {
        public List<IQuestion<T>> questions;
        public Dictionary<IUser, int> usersScores;

        public TriviaData(List<IQuestion<T>> questions = null)
        {
            this.questions = questions != null ? new List<IQuestion<T>>(questions) : new List<IQuestion<T>>();
            usersScores = new Dictionary<IUser, int>();
        }
    }
}
