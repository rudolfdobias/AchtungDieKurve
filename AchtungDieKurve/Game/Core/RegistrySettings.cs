using Newtonsoft.Json;
using System;
using System.IO;

namespace AchtungDieKurve.Game.Core
{
    public enum RegistrySection { Core, Players }

    public class RegistrySettings
    {
        private const string GameFolderName = "AchtungDieKurve";
    
        public static void Write<T>(string key, T content)
        {
            var path = Path.Combine(GetRootPath(), GameFolderName);
            Directory.CreateDirectory(path);

            var file = Path.Combine(path, $"{key}.json");

            File.WriteAllText(file, JsonConvert.SerializeObject(content));
        }

        public static T Read<T>(string key) where T:class
        {
            var content = GetFileContent(key);
            
            if (string.IsNullOrEmpty(content))
                return default;

            return JsonConvert.DeserializeObject<T>(content);
        }

        private static string GetFileContent(string key)
        {
            var path = Path.Combine(GetRootPath(), GameFolderName, $"{key}.json");
            if (!File.Exists(path))
                return null;

            return File.ReadAllText(path);
        }

        private static string GetRootPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }


}