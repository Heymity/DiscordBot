using DiscordBot.Utilities.Managers.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Managers.Storage
{
    public static class DataStorageManager
    {
        public static IDictionary<ulong, GuildDataManager> GuildsData { get; private set; }
    }
}
