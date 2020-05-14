using UnityEngine;
using System.Collections;
using System;

public class DummyBehaviour : Behaviour 
{
    float totalDamage = 0;
    float dps = 0;
    float damageStartTime = 0;
    float noDamageTime = 0;
    private void Start() {
        self.OnDamaged += AddDamage;
        self.OnDead += Revive;
    }
    private void Update() {
        if(noDamageTime > 0) noDamageTime = Mathf.Max(noDamageTime - Time.deltaTime,0);
        else 
        {
            damageStartTime = Time.realtimeSinceStartup;
            totalDamage = 0;
            self.Heal(self,self.Status.maxHealth);
        }
    }

    private void OnGUI() {
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        var rect = new Rect(new Vector2(pos.x - 120,Screen.height - pos.y - 60),new Vector2(240,30));
        GUI.Box(rect,"Total : " + totalDamage.ToString() + " / DPS : " + dps.ToString() + " per sec");
    }

    private void AddDamage(Character self, Character source, float amount)
    {
        dps = totalDamage / (Time.realtimeSinceStartup - damageStartTime);
        totalDamage += amount;
        noDamageTime = 1;
    }

    void Revive(Character self, string cause, ref bool destroyObject)
    {
        destroyObject = false;
        self.Heal(self,self.Status.maxHealth);
    }
}