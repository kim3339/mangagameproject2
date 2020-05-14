using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using JetBrains;

public class Game : MonoBehaviour
{
    public static Game instance;
    
    [SerializeField]
    TextAsset InitLanguage;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null) instance = this;
        else Destroy(gameObject);

        TextLocalizer.LoadResource();
        
    }

    

    public static Projectile CreateProjectile(GameObject prefab, Vector3 pos, int targetLayerMask)
    {
        if (prefab.GetComponent<Projectile>() == null) return null;
        
        var ins = Instantiate(prefab).GetComponent<Projectile>();
        ins.transform.position = pos;
        ins.info.targetLayerMask = targetLayerMask;
        return ins;
    }

    public static void Explode()
    {

    }
}
