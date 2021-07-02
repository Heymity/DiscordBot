using Discord;
using System.Collections.Generic;

namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public struct TriviaData<T> where T : BaseAnswer
    {
        public List<BaseQuestion> questions;
        private bool shoudStoreUserData;
        private Dictionary<ulong, TriviaUserData> usersScores;

        public TriviaData(bool _shoudStoreScores, List<BaseQuestion> questions = null)
        {
            this.questions = questions != null ? new List<BaseQuestion>(questions) : new List<BaseQuestion>();
            shoudStoreUserData = _shoudStoreScores;
            usersScores = new Dictionary<ulong, TriviaUserData>();
        }

        public int AddScore(IUser user, int score) => AddScore(user.Id, score);

        public int AddScore(ulong user, int score)
        {
            if (!shoudStoreUserData) return -1;
            usersScores ??= new Dictionary<ulong, TriviaUserData>();

            if (usersScores.ContainsKey(user))
                return usersScores[user].AddScore(score);
            else
                usersScores.Add(user, new TriviaUserData(user, score, 1, 1));
            return score;
        }

        public TriviaUserData QuestionSolved(IUser user, bool wasCorrect, int score)
            => QuestionSolved(user.Id, wasCorrect, score);

        public TriviaUserData QuestionSolved(ulong user, bool wasCorrect, int score)
        {
            if (!shoudStoreUserData) return null;
            usersScores ??= new Dictionary<ulong, TriviaUserData>();

            if (usersScores.ContainsKey(user))
                return usersScores[user].QuestionSolved(wasCorrect, score);
            else
                usersScores.Add(user, new TriviaUserData(user, score, 1, wasCorrect ? 1 : 0));
            return usersScores[user];
        }

        public TriviaUserData GetUserData(ulong Id)
        {
            if (!shoudStoreUserData) return null;
            usersScores ??= new Dictionary<ulong, TriviaUserData>();

            if (usersScores.ContainsKey(Id))
                return usersScores[Id];

            return null;
        }

        [System.Serializable]
        public class TriviaUserData
        {
            public ulong id;
            public int score;
            public int answered;
            public int answeredCorrectly;

            public TriviaUserData(ulong id, int score, int answered, int answeredCorrectly)
            {
                this.id = id;
                this.score = score;
                this.answered = answered;
                this.answeredCorrectly = answeredCorrectly;
            }

            public int AddScore(int scoreToAdd) => score += scoreToAdd;

            public TriviaUserData QuestionSolved(bool wasCorrect, int score)
            {
                answered++;
                if (wasCorrect)
                {
                    answeredCorrectly++;
                    this.score += score;
                }

                return this;
            }


        }
    }
}