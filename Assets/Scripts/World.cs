using UnityEngine;
using System.Collections;
using System;

public class World : MonoBehaviour
{
    public static World instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public Projectile CreateProjectile(GameObject prefab, Vector3 pos, int targetLayerMask)
    {
        if (prefab.GetComponent<Projectile>() == null) return null;
        
        var ins = Instantiate(prefab).GetComponent<Projectile>();
        ins.transform.position = pos;
        ins.info.targetLayerMask = targetLayerMask;
        return ins;
    }

    public void Explode()
    {

    }
}
