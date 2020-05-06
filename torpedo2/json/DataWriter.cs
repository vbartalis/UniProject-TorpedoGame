using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using torpedo2.data;

namespace torpedo2.json
{
    class DataWriter
    {

        public IList<GameOutcome> ReadJson()
        {
            string path = GetAppDataPath();
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<IList<GameOutcome>>(jsonString);
                //return JsonSerializer.Deserialize<IList<GameOutcome>>(jsonString);
            }
            else
            {
                return new List<GameOutcome>();
            }
        }

        public string ReadJsonString()
        {
            string path = GetAppDataPath();
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return jsonString;
            }
            else
            {
                return "No records";
            }
            
        }

        public void UpdateJson(IList<GameOutcome> gameOutcomes)
        {
            string path = GetAppDataPath();
            string jsonString = JsonConvert.SerializeObject(gameOutcomes);
            //string jsonString = JsonSerializer.Serialize(gameOutcomes);
            File.WriteAllText(path, jsonString);
        }

        public static string GetAppDataPath()
        {
            var localAppFolder = GetLocalFolder();
            var appDataPath = Path.Combine(localAppFolder, "SavedScores.json");
            return appDataPath;
        }

        public static string GetLocalFolder()
        {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localAppFolder = Path.Combine(localAppDataFolder, "MyTorpedoGames");

            if (!Directory.Exists(localAppFolder))
            {
                Directory.CreateDirectory(localAppFolder);
            }

            return localAppFolder;
        }
    }
}
