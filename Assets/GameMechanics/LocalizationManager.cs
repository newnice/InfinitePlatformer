using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {
    private static Dictionary<GameLanguage, Dictionary<string, string>> _localization =
        new Dictionary<GameLanguage, Dictionary<string, string>>();

    public static IEnumerable<GameLanguage> AvailableLanguages { get; } = _localization.Keys;

    public static string TranslateFor(GameLanguage language, string key) {
        if (!_localization.TryGetValue(language, out var dictionary)) {
            Debug.LogError($"Language {language} not found");
            return "Not found";
        }

        if (!dictionary.TryGetValue(key, out var localName)) {
            Debug.LogError($"Localization for {key} not found in dictionary {language}");
        }

        return localName;
    }

    static LocalizationManager() {
        if (TryGetLocalizationFiles(out var files))
            InitLocalizationDictionaries(files);
    }

    private static void InitLocalizationDictionaries(IList<FileInfo> files) {
        foreach (var file in files) {
            var language = new GameLanguage(file.Name.Substring(0, file.Name.IndexOf(".csv")));
            var single = ReadLocalization(file.FullName);
            _localization.Add(language, single);
        }
    }

    private static Dictionary<string, string> ReadLocalization(string filePath) {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        using (StreamReader sr = File.OpenText(filePath)) {
            string line;
            while ((line = sr.ReadLine()) != null) {
                var values = line.Split(';');
                if (values.Length < 2) {
                    Debug.LogError($"not found values in dictionary '{filePath}' in line '{line}'");
                    continue;
                }

                if (!dictionary.ContainsKey(values[0]))
                    dictionary.Add(values[0], values[1]);
                else {
                    Debug.LogError($"Duplicated key'{values[0]}' in dictionary '{filePath}'");
                }
            }
        }

        return dictionary;
    }

    private static bool TryGetLocalizationFiles(out IList<FileInfo> fileNames) {
        var di = new DirectoryInfo(@"Assets/Resources/languages");
        fileNames = new List<FileInfo>(di.GetFiles("*.csv"));

        return fileNames.Count > 0;
    }
}