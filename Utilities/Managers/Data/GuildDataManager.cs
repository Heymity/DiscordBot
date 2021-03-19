using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Managers.Data
{
    public class GuildDataManager
    {
        public IDictionary<ulong, ChannelDataManager> ChannelsData { get; private set; }


    }
}
