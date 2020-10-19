using Newtonsoft.Json;
using OTIPChanger.Models;
using System;
using System.IO;

namespace OTIPChanger.Classes
{
    public static class ConfigurationHandler
    {
        private static string ConfigPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/OTIPChanger/config.json";
        private static Configuration _Configuration { get; set; }

        public static void SetPath(string path)
        {
            _Configuration.Path = path;
        }

        public static string GetPath()
        {
            return _Configuration.Path;
        }

        public static void SetSaveAsNewFile(bool saveAsNewFile)
        {
            _Configuration.SaveAsNewFile = saveAsNewFile;
        }

        public static bool GetSaveAsNewFile()
        {
            return _Configuration.SaveAsNewFile;
        }

        public static void SaveConfiguration()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_Configuration));
            }
            catch
            {
                //
            }
        }

        public static bool LoadConfiguration()
        {
            try
            {
                _Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath));
            }
            catch
            {
                _Configuration = new Configuration();
                return false;
            }

            return true;
        }
    }
}
