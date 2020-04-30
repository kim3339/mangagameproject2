using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo_1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, string> sample = new Dictionary<string, string>();
        sample.Add("New Game", "New Game");
        sample.Add("Load Game", "Load Game");
        sample.Add("Settings", "Settings");
        sample.Add("Exit Game", "Exit Game");
        JSONObject json = new JSONObject(sample);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/en_Message.json", json.Print(true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
