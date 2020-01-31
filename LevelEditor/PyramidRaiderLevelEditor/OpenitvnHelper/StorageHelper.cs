#if WINDOWS_PHONE
using System.IO.IsolatedStorage;
#endif
#if WINDOWS
using System.IO;
#endif

namespace OpenitvnGame.Helper
{
    public class StorageHelper
    {
#if WINDOWS_PHONE
        public static bool StoreSetting(string key, object value, bool overwrite)
        {
            if (overwrite || IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
                return true;
            }

            return false;
        }

        public static T GetSetting<T>(string key)
        {

            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                return (T)IsolatedStorageSettings.ApplicationSettings[key];
            }

            return default(T);
        }

        public static T GetSetting<T>(string key, T defaultVal)
        {

            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                return (T)IsolatedStorageSettings.ApplicationSettings[key];
            }

            return defaultVal;
        }

        public static void RemoveSetting(string key)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove(key);
        }

        public static void SaveSetting()
        {
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
#endif

#if WINDOWS
        private static string storageFolder = "save";

        public static bool StoreSetting(string key, object value, bool overwrite)
        {
            try
            {
                string path = "/" + storageFolder + "/" + key;
                if (overwrite || File.Exists(path))
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.Write(value.ToString());
                        writer.Close();
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static T GetSetting<T>(string key, T defaultVal)
        {
            string path = "/" + storageFolder + "/" + key;
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string content = reader.ReadToEnd();
                        reader.Close();
                        return defaultVal;
                    }
                }
                catch { }
            }

            return defaultVal;
        }

        public static void RemoveSetting(string key)
        {
        }

        public static void SaveSetting()
        {
        }
#endif

#if ANDROID
        public static bool StoreSetting(string key, object value, bool overwrite)
        {
            return false;
        }

        public static T GetSetting<T>(string key, T defaultVal)
        {
            return defaultVal;
        }

        public static void RemoveSetting(string key)
        {
        }

        public static void SaveSetting()
        {
        }
#endif
    }
}
