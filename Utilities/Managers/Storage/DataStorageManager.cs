using DiscordBot.Utilities.Managers.Data;
using DiscordBot.Utilities.Trivia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace DiscordBot.Utilities.Managers.Storage
{
    [Serializable]
    public class DataStorageManager
    {
        private static DataStorageManager _current;
        public static DataStorageManager Current
        {
            get => _current ??= new DataStorageManager();
            private set => _current = value;
        }

        public DataStorageManager()
        {
            GuildsData = new Dictionary<ulong, GuildDataManager>();
            GeneralTriviaData = new TriviaData<BaseAnswer>(null);
            _current = this;
        }

        public Dictionary<ulong, GuildDataManager> GuildsData { get; private set; }
        public TriviaData<BaseAnswer> GeneralTriviaData { get; set; }

        public IQuestion<BaseAnswer> GetRandomQuestion(ulong guildId)
        {
            var tmp = new List<IQuestion<BaseAnswer>>();
            var tmp2 = GetOrCreateGuild(guildId).GuildTriviaData.questions;
            if (tmp2 != null) tmp.AddRange(tmp2);
            tmp.AddRange(GeneralTriviaData.questions);

            Random rng = new Random();
            return tmp[rng.Next() % tmp.Count];
        }

        public GuildDataManager GetOrCreateGuild(ulong guildId)
        {
            if (GuildsData.ContainsKey(guildId))
            {
                return GuildsData[guildId];
            } else
            {
                GuildsData.Add(guildId, new GuildDataManager(guildId));
                return GuildsData[guildId];
            }
        } 

        public void SaveData()
        {
            FileStream fs = new FileStream("DataFile.dat", FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _current);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public void LoadData()
        {
            FileStream fs = new FileStream("DataFile.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                _current = (DataStorageManager)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
