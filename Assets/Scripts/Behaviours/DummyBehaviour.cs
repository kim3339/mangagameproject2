using UnityEngine;
using System.Collections;
using System;

public class DummyBehaviour : Behaviour 
{
    float totalDamage = 0;
    float dps = 0;

    float noDamageTime = 0;
    private void Awake() {
    }
    private void Update() {
        if(noDamageTime > 0) noDamageTime = Mathf.Max(noDamageTime - Time.deltaTime,0);
        else totalDamage = 0;
    }

    private void OnGUI() {
        var rect = new Rect(Camera.main.WorldToScreenPoint(transform.position),new Vector2(1,0.5f));
        GUI.Box(rect,"Total : " + totalDamage.ToString());
    }

    private void AddDamage(Character self, Character source, float amount)
    {
        totalDamage += amount;
        noDamageTime = 1;
    }
}