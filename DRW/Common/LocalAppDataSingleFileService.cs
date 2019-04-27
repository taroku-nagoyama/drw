using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miktemk.Services
{
    /// <summary>
    /// Maintains a backup of one piece of data in a JSON file under `%LocalAppData%\miktemk\ProjName\Filename`
    /// </summary>
    /// <typeparam name="T">type of data serialized in the target file</typeparam>
    public abstract class LocalAppDataSingleFileService<T>
        where T : class, new()
    {
        protected abstract string ProjName { get; }
        protected abstract string Filename { get; }

        private string LocalAppDataFilename => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "miktemk",
            ProjName,
            Filename
            );

        public T LoadOrCreateNew()
        {
            if (!File.Exists(LocalAppDataFilename))
                return Save(new T());
            return LoadFromFile<T>(LocalAppDataFilename);
        }

        public void ModifyAndSave(Action<T> callback)
        {
            var appState = LoadOrCreateNew();
            callback(appState);
            Save(appState);
        }

        protected T Save(T data)
        {
            if (!File.Exists(LocalAppDataFilename))
            {
                var dirPath = Path.GetDirectoryName(LocalAppDataFilename);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
            WriteToFile(data, LocalAppDataFilename);
            return data;
        }

        public static T LoadFromFile<T>(string filename) where T : class
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
        }
        public static void WriteToFile<T>(T data, string filename, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            string json = JsonConvert.SerializeObject(data, formatting);
            File.WriteAllText(filename, json);
        }
    }
}
