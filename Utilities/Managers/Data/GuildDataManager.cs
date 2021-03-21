using DiscordBot.Utilities.Trivia;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Managers.Data
{
    [System.Serializable]
    public class GuildDataManager
    {
        public Dictionary<ulong, ChannelDataManager> ChannelsData { get; private set; }
        public TriviaData<BaseAnswer> GuildTriviaData { get; set; }
    }
}
