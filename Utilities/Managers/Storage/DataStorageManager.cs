using DiscordBot.Utilities.Managers.Data;
using DiscordBot.Utilities.Trivia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Timers;

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

        public GuildDataManager this[ulong i] 
        { 
            get => GetOrCreateGuild(i);
            set
            {
                if (GuildsData.ContainsKey(i)) GuildsData[i] = value;
                else GuildsData.Add(i, value);
            }
        }
        

        public Dictionary<ulong, GuildDataManager> GuildsData { get; private set; }
        public TriviaData<BaseAnswer> GeneralTriviaData { get; set; }


        public DataStorageManager()
        {
            GuildsData = new Dictionary<ulong, GuildDataManager>();
            GeneralTriviaData = new TriviaData<BaseAnswer>(false);
            _current = this;
            AutoSaveManager.Start();
        }


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

    public static class AutoSaveManager
    {
        public const double SAVE_INTERVAL = 10000; // 5 minutes = 300000 10 minutes = 600000
        const double SECOND = 1000;

        public static Dictionary<ChangePriority, double> PriorityEffect { get; set; }

        public static Timer SaveSchedule { get; set; }

        public static void Start()
        {
            SaveSchedule = new Timer()
            {
                Interval = SAVE_INTERVAL,
                AutoReset = true,
            };
            SaveSchedule.Elapsed += AutoSave;
            SaveSchedule.Start();

            PriorityEffect = new Dictionary<ChangePriority, double>()
            {
                { ChangePriority.ImmediateSave, SAVE_INTERVAL },
                { ChangePriority.UserDataChange, 30 * SECOND },
                { ChangePriority.ImportantUserDataChange, 60 * SECOND },
                { ChangePriority.GuildTriviaChange, 30 * SECOND },
                { ChangePriority.TriviaQuestionAdded, 240 * SECOND },
                { ChangePriority.ChannelDataChange, 60 * SECOND },
                { ChangePriority.GuildDataChange, 120 * SECOND },
                { ChangePriority.GeneralDataChange, 300 * SECOND }
            };
        }

        private static void AutoSave(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Auto Saving...");
            DataStorageManager.Current.SaveData();
            SaveSchedule.Interval = SAVE_INTERVAL;
        }

        public static void ReduceIntervalByChangePriority(ChangePriority priority)
        {

        }
    }

    [Flags]
    public enum ChangePriority
    {
        ImmediateSave = 0,            // 0x0000_0000,
        UserDataChange = 1,           // 0x0000_0001,
        ImportantUserDataChange = 2,  // 0x0000_0010,
        GuildTriviaChange = 4,        // 0x0000_0100,
        TriviaQuestionAdded = 8,      // 0x0000_1000,
        ChannelDataChange = 16,       // 0x0001_0000,
        GuildDataChange = 32,         // 0x0010_0000,
        GeneralDataChange = 64        // 0x0100_0000,
    }
}
