using DiscordBot.Utilities.Trivia;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Managers.Data
{
    [System.Serializable]
    public class GuildDataManager 
    {
        public ulong Id { get; set; }
        public Dictionary<ulong, ChannelDataManager> ChannelsData { get; private set; }
        public TriviaData<BaseAnswer> GuildTriviaData { get; set; }

        public GuildDataManager(ulong id, Dictionary<ulong, ChannelDataManager> channelsData, TriviaData<BaseAnswer> guildTriviaData)
        {
            Id = id;
            ChannelsData = channelsData ?? new Dictionary<ulong, ChannelDataManager>();
            GuildTriviaData = guildTriviaData;
        }

        public GuildDataManager(ulong id)
        {
            Id = id;
            ChannelsData = new Dictionary<ulong, ChannelDataManager>();
            GuildTriviaData = new TriviaData<BaseAnswer>(true);
        }
    }
}
