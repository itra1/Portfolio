using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


    public class LocalizationManager
    {
        public const string DefaultLocale = "English";
        public const string LocalizationFilesPath = "Localization";
        private static LocalizationManager _instance = null;
        private Dictionary<string, string> localization = new Dictionary<string, string>();

        private LocalizationManager()
        {
            LoadLocalization(GetSystemLocale());
        }

        public string CurrentLocale { get; set; }

        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                }

                return _instance;
            }
        }
        
        public static string GetText(string id)
        {
            return Instance.GetLocalizedText(id);
        }

        public static string GetSystemLocale()
        {
            var locale = "";

            switch (Application.systemLanguage)
            {
                case SystemLanguage.English: locale = "English"; break;
                case SystemLanguage.Russian: locale = "Russian"; break;
                case SystemLanguage.Unknown: locale = "English"; break;
                default: locale = "English"; break;
            }

            return locale;
        }
        
        public void LoadLocalization(string locale)
        {
      return;
            if (DevMode.IsEnabled) locale = "Test";
            
            MemoryStream localizationFileStream = null;

            if (PlayerPrefs.HasKey("CustomLocalization"))
            {
                localizationFileStream = new MemoryStream(Encoding.UTF8.GetBytes(
                    PlayerPrefs.GetString("CustomLocalization", "")));
            }
            else
            {
                var localizationFile = Resources.Load(LocalizationFilesPath) as TextAsset;
                localizationFileStream = new MemoryStream(localizationFile.bytes);
            }

            using (var reader = new CsvFileReader(localizationFileStream))
            {
                var row = new CsvRow();
                reader.ReadRow(row);

                var localeIndex = row.IndexOf(locale);
                if (localeIndex == -1)
                    localeIndex =
                        row.IndexOf(DefaultLocale); // it means system locale not found so english used instead
                while (reader.ReadRow(row))
                {
                    var localizedString = localeIndex < row.Count ? row[localeIndex] : row[0];
#if UNITY_EDITOR
                    if (localization.ContainsKey(row[0]))
                    {
                        it.Logger.Log("Double key " + row[0]);
                    } 
#endif
                    localization.Add(row[0], localizedString.Replace("\\n", "\n").Replace("%", "\""));
                }
            }

            localizationFileStream.Close();

            CurrentLocale = locale;
        }

        private string GetLocalizedText(string id)
        {
            if (localization.ContainsKey(id))
            {
                if (localization[id] != null)
                {
                    return localization[id];
                }
            }

            return "[n/l]" + id; // Not localized
        }
    }

    public static class LocalizationExtentions
    {
        /// <summary>
        ///
        /// </summary>
        public static string Localized(this string self)
        {
            return I2.Loc.LocalizationManager.GetTermTranslation(self);
        }
    }