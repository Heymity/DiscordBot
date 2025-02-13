﻿using DiscordBot.Utilities.Trivia;
using System.Collections.Generic;

namespace DiscordBot.Utilities.Managers.Data
{
    [System.Serializable]
    public class GuildDataManager
    {
        public ulong Id { get; set; }

        private char commandPrefix = '!';
        public char CommandPrefix { get => commandPrefix == '\0' ? '!' : commandPrefix; set => commandPrefix = value; }
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