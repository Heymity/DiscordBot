using Discord;
using System.Collections.Generic;

namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public struct TriviaData<T> where T : BaseAnswer
    {
        public List<IQuestion<T>> questions;
        public bool shouldStoreScores;
        public Dictionary<IUser, int> usersScores;
        
        public TriviaData(bool _shoudStoreScores, List<IQuestion<T>> questions = null)
        {
            this.questions = questions != null ? new List<IQuestion<T>>(questions) : new List<IQuestion<T>>();
            shouldStoreScores = _shoudStoreScores;
            usersScores = new Dictionary<IUser, int>();
        }

        public int AddScore(IUser user, int score)
        {
            if (!shouldStoreScores) return -1;
            usersScores ??= new Dictionary<IUser, int>();

            if (usersScores.ContainsKey(user))
                return usersScores[user] += score;
            else
                usersScores.Add(user, score);
            return score;
        }
    }
}
