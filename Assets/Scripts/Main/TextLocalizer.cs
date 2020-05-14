using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using JetBrains;

public class TextLocalizer : MonoBehaviour {
    public static string CurrentLanguageName;
    public static string DefaultLanguageName;
    public static List<string> LanguageNames = new List<string>();
    static Dictionary<string,string> LanguageDic;
    static Dictionary<string,string> defaultLanguage;
    static List<Dictionary<string,string>> Languages = new List<Dictionary<string, string>>();
    static Action onLanguageChange;

    public string key;
    public string localizedString;
    public Text textComponent;

    public static void LoadResource(string defaultLanguageName = "")
    {
        LanguageNames.Clear();
        Languages.Clear();
        var filenames = Directory.GetFiles(Application.streamingAssetsPath + "/languages","*.json");
        foreach(var filename in filenames)
        {
            var lang = JSONObject.Create(new StreamReader(Application.streamingAssetsPath + "/languages/" + filename).ReadLine()).ToDictionary();
            if(lang.ContainsKey("language_name"))
            {
                Languages.Add(lang);
                string n;
                lang.TryGetValue("language_name",out n);
                LanguageNames.Add(n);
            }
        }

        if(defaultLanguageName == "")
            Languages[0].TryGetValue("language_name",out defaultLanguageName);
        
        if(ChangeLanguage(defaultLanguageName))
        {
            Languages[0].TryGetValue("language_name",out defaultLanguageName);
            ChangeLanguage(defaultLanguageName);
        }
        defaultLanguage = LanguageDic;
        DefaultLanguageName = defaultLanguageName;
    }
    public static bool ChangeLanguage(string name)
    {
        var langDic = Languages.Find((dic) => 
        {
            string thisname;
            dic.TryGetValue("language_name",out thisname);
            return name == thisname;
        });
        if (langDic == null) return false;
        LanguageDic = langDic;

        CurrentLanguageName = name;
        onLanguageChange?.Invoke();
        return true;
    }
    
    public static bool ChangeDefaultLanguage(string name)
    {
        var langDic = Languages.Find((dic) => 
        {
            string thisname;
            dic.TryGetValue("language_name",out thisname);
            return name == thisname;
        });
        if (langDic == null) return false;
        defaultLanguage = langDic;
        DefaultLanguageName = name;
        return true;
    }
    private void Awake() {
        if(textComponent == null) textComponent = GetComponent<Text>();
        void LocalizeText()
        {
            string localized;
            if(LanguageDic.ContainsKey(key))
            {
                LanguageDic.TryGetValue(key,out localized);
            }
            else
            {
                defaultLanguage.TryGetValue(key,out localized);
            }
            localizedString = localized;

            if(textComponent != null)
            {
                textComponent.text = localizedString;
            }
        }
        onLanguageChange += LocalizeText;
    }
}