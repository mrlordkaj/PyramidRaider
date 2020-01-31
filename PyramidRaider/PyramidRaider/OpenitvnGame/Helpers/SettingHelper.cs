#if WINDOWS_PHONE
using System.IO.IsolatedStorage;
#endif

#if WINDOWS || ANDROID
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif

#if ANDROID
using Android.App;
using Android.Content;
using Microsoft.Xna.Framework;
using System;
#endif

namespace OpenitvnGame.Helpers
{
    public class SettingHelper
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

#if ANDROID
        private const string SETTING_FILE = "__LocalSettings";

        static ISharedPreferences _sharedPrefs;
        private static ISharedPreferences SharedPrefs
        {
            get
            {
                if (_sharedPrefs == null) _sharedPrefs = Game.Activity.GetSharedPreferences(SETTING_FILE, FileCreationMode.Private);
                return _sharedPrefs;
            }
        }

        static Dictionary<string, object> _settingData;
        public static Dictionary<string, object> SettingData
        {
            get
            {
                if (_settingData == null)
                {
                    _settingData = new Dictionary<string, object>();
                    try
                    {
                        byte[] b = Convert.FromBase64String(SharedPrefs.GetString(SETTING_FILE, string.Empty));
                        IFormatter formatter = new BinaryFormatter();
                        Stream reader = new MemoryStream(b);
                        reader.Seek(0, SeekOrigin.Begin);
                        _settingData = (Dictionary<string, object>)formatter.Deserialize(reader);
                    }
                    catch { }
                }
                return _settingData;
            }
        }
#endif
#if WINDOWS
        private const string SETTING_FILE = @"Save\settings.dat";

        static Dictionary<string, object> _settingData;
        public static Dictionary<string, object> SettingData
        {
            get
            {
                if (_settingData == null)
                {
                    _settingData = new Dictionary<string, object>();
                    try
                    {
                        IFormatter Formatter = new BinaryFormatter();
                        FileStream writer = new FileStream(SETTING_FILE, FileMode.Open);
                        _settingData = (Dictionary<string, object>)Formatter.Deserialize(writer);
                    }
                    catch { }
                }
                return _settingData;
            }
        }
#endif

#if WINDOWS || ANDROID
        public static bool StoreSetting(string key, object value, bool overwrite)
        {
            if (overwrite || SettingData.ContainsKey(key))
            {
                SettingData[key] = value;
                return true;
            }
            return false;
        }

        public static T GetSetting<T>(string key)
        {
            if (SettingData.ContainsKey(key))
            {
                return (T)SettingData[key];
            }
            return default(T);
        }

        public static T GetSetting<T>(string key, T defaultVal)
        {
            if (SettingData.ContainsKey(key))
            {
                return (T)SettingData[key];
            }
            return defaultVal;
        }

        public static void RemoveSetting(string key)
        {
            if (SettingData.ContainsKey(key)) SettingData.Remove(key);
        }
#endif

#if ANDROID
        public static void SaveSetting()
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream writer = new MemoryStream();
            formatter.Serialize(writer, SettingData);
            writer.Flush();
            writer.Position = 0;
            ISharedPreferencesEditor editor = SharedPrefs.Edit();
            editor.PutString(SETTING_FILE, Convert.ToBase64String(writer.ToArray()));
            editor.Commit();
        }
#endif
#if WINDOWS
        public static void SaveSetting()
        {
            IFormatter formatter = new BinaryFormatter();
            if(File.Exists(SETTING_FILE)) File.Delete(SETTING_FILE);
            FileStream writer = new FileStream(SETTING_FILE, FileMode.Create);
            formatter.Serialize(writer, SettingData);
        }
#endif
    }
}
