using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Localizer
{
    public static Dictionary<string, string> Message;
    public static Dictionary<string, string> Script;
    public void Localize(string name)
    {
        string rawjson = File.ReadAllText(Application.persistentDataPath + "/" + name + "_Message.json");
        JSONObject obj = new JSONObject(rawjson);
        Message = obj.ToDictionary();

        rawjson = File.ReadAllText(Application.persistentDataPath + "/" + name + "_Script.json");
        obj = new JSONObject(rawjson);
        Script = obj.ToDictionary();
    }
}
