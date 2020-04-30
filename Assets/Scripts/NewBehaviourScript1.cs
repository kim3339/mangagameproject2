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

    public Projectile CreateProjectile(Vector3 position ,GameObject prefab ,Action<Projectile> updateFunc, ProjectileInfo info)
    {
        if (prefab.GetComponent<Projectile>() == null) return null;

        

        var ins = Instantiate(prefab).GetComponent<Projectile>();
        ins.Initialize(info, updateFunc);
        return ins;
    }

    public void Explode()
    {

    }
}
