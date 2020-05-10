using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    public Action<Projectile> OnUpdate;
    public float LifeTime => lifetime;
    [NonSerialized]    

    float lifetime = 0;
    Vector2 move;
    public ProjectileInfo info;
    CircleCollider2D collider;
    List<Character> prevhit = new List<Character>();
    [SerializeField]
    GameObject onCreatePrefab;
    [SerializeField]
    GameObject onDeathPrefab;

    
    private void Awake() 
    {
        collider = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        if(onCreatePrefab != null)
        {
            if(onCreatePrefab.GetComponent<ParticleSystem>()!= null)
            {
                var t = onCreatePrefab.transform;
                var ins = Instantiate(onCreatePrefab,t.position + transform.position,t.rotation * transform.rotation);
                Destroy(ins,ins.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }

    private void Update()
    {
        

        lifetime += Time.deltaTime;
        OnUpdate?.Invoke(this);
        if (info.duration != 0)
        {
            if (info.duration < lifetime) Destroy();
        }

        var bounds = collider.bounds;
        

        var hits = Physics2D.CircleCastAll(bounds.center,
                                           collider.radius,
                                           move.normalized,
                                           move.magnitude,
                                           info.targetLayerMask | (1 << 8));
        var currentHit = new List<Character>();
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider.gameObject.layer == 8)
            {
                if(!info.ignorePlatform)
                {
                    transform.position = hits[i].centroid;
                    Destroy();
                    return;
                }
            }
            else
            {
                var ins = hits[i].collider.GetComponent<Character>();
                if(ins == null) continue;
                if(prevhit.Contains(ins)) continue;
                Hit(ins);
                currentHit.Add(ins);
                if(info.fierceCount-- <= 0)
                {
                    transform.position = hits[i].centroid;
                    Destroy();
                    return;
                }
            }
        }
        transform.position += (Vector3)move;
        
        prevhit.Clear();
        prevhit.AddRange(currentHit);

        float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        move = Vector2.zero;
    }


    public void Move(Vector2 moveAmount)
    {
        move += moveAmount;
    }

    protected virtual void Hit(Character target)
    {
        target.Damage(info.source, info.damage);
    }
    public virtual void Destroy()
    {
        if(onDeathPrefab != null)
        {
            if(onDeathPrefab.GetComponent<ParticleSystem>()!= null)
            {
                var t = onDeathPrefab.transform;
                var ins = Instantiate(onDeathPrefab,t.position + transform.position,t.rotation * transform.rotation);
                Destroy(ins,ins.GetComponent<ParticleSystem>().main.duration);
            }
        }
        Destroy(gameObject);
    }

}
