using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Settings
{
    public static bool IsMusic;
    public static bool IsSounds;
    public static string Language;

    public struct SettingsValues
    {
        public bool IsMusic;
        public bool IsSounds;
        public string Language;
        public SettingsValues(bool isMusic, bool isSounds, string language)
        {
            IsMusic = isMusic;
            IsSounds = isSounds;
            Language = language;
        }
    }

    public static void InitializeSettings() 
    { 
        if (!File.Exists(Application.persistentDataPath + "/Settings.json"))
        {
            IsMusic = true;
            IsSounds = true;
            Language = "en";
            WriteSettings();
        }
        else
        {
            ReadSettings();
        }
    }

    public static void WriteSettings()
    {
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/Settings.json"))
        {
            string SerializeConfig = JsonConvert.SerializeObject(new SettingsValues(IsMusic, IsSounds, Language));
            sw.Write(SerializeConfig);
        }
    }

    private static void ReadSettings()
    {
        using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/Settings.json"))
        {
            if (sr != null)
            {
                var settingsValues = JsonConvert.DeserializeObject<SettingsValues>(sr.ReadToEnd());
                IsMusic = settingsValues.IsMusic;
                IsSounds = settingsValues.IsSounds;
                Language = settingsValues.Language;
            }
        }
    }
}
