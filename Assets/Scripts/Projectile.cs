using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    Action<Projectile> OnUpdate;
    public ProjectileInfo Info => info;
    public float LifeTime => lifetime;
    public ProjectileController controller;    

    float lifetime = 0;

    [SerializeField]
    ProjectileInfo info;

    List<Character> hitted = new List<Character>();
    
    private void Awake()
    {
        controller = GetComponent<ProjectileController>();
    }
    private void Update()
    {
        lifetime += Time.deltaTime;
        OnUpdate?.Invoke(this);
        if (info.duration != 0)
        {
            if (info.duration < lifetime) Destroy();
        }
    }

    public void Initialize(ProjectileInfo info, Action<Projectile> updateFunc)
    {
        this.info = info;
        if (updateFunc != null)
            OnUpdate = updateFunc;
        StartCoroutine(HitManage());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8 && !info.ignorePlatform)
        {
            Destroy();
            return;
        }
        if ((collision.gameObject.layer & info.targetLayerMask) == 0) return;
        var _target = collision.gameObject.GetComponent<Character>();
        if (_target != null) hitted.Add(_target);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.layer & info.targetLayerMask) == 0) return;
        var _target = collision.gameObject.GetComponent<Character>();
        if (_target != null) hitted.Remove(_target);
    }
    
    IEnumerator HitManage()
    {
        var wait = new WaitWhile(() => hitted.Count == 0);
        while(true)
        {
            yield return wait;

            for (int i = 0; i < hitted.Count; i--)
            {
                Hit(hitted[i]);
                
                if (info.fierceCount == 0)
                {
                    Destroy();
                    break;
                }
                else info.fierceCount--;
            }
        }
    }

    protected virtual void Hit(Character target)
    {
        target.Damage(info.source, info.damage);
    }
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

}
