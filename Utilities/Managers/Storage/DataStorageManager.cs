using DiscordBot.Utilities.Managers.Data;
using DiscordBot.Utilities.Trivia;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Managers.Storage
{
    [System.Serializable]
    public static class DataStorageManager
    {
        public static Dictionary<ulong, GuildDataManager> GuildsData { get; private set; }
        public static TriviaData<BaseAnswer> GeneralTriviaData { get; set; }

        public static IQuestion<BaseAnswer> GetRandomQuestion(ulong guildId)
        {
            var tmp = new List<IQuestion<BaseAnswer>>(GeneralTriviaData.questions);
            tmp.AddRange(GuildsData[guildId].GuildTriviaData.questions);

            Random rng = new Random();
            return tmp[rng.Next() % tmp.Count];
        }
    }
}
