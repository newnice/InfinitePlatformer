using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct GameLanguage {
    public string Name;

    public static GameLanguage Empty= new GameLanguage("Empty");
    public GameLanguage(string name) {
        Name = name;
    }

    public override string ToString() {
        return $"Language {Name} ";
    }

    public override bool Equals(object obj) {
        if (!(obj is GameLanguage)) return false;
        var gl = (GameLanguage) obj;
        return Name.Equals(gl.Name);
    }

    public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;
}


[CustomPropertyDrawer(typeof(GameLanguage))]
public class LanguageDrawer : PropertyDrawer {
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var availableLanguages = new List<GameLanguage>(LocalizationManager.AvailableLanguages);

        var contents = availableLanguages.Select(l => new GUIContent(l.Name)).ToArray();
        var nameValue = property.FindPropertyRelative("Name");
        var index = nameValue != null ? availableLanguages.FindIndex(gl => gl.Name.Equals(nameValue.stringValue)) : 0;

        index = index < 0 ? 0 : index;
        var newSelected = EditorGUI.Popup(position, index, contents);
        var language = availableLanguages[newSelected];
        if (nameValue != null)
            nameValue.stringValue = language.Name;
    }
}