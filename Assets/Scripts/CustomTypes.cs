using System;
using UnityEngine;

public delegate void OnHealthChangedHandler(Character self, Character source, float amount);
public delegate void OnDeadHandler(Character self, string cause, ref bool destroyObject);
public delegate void OnKnockbackedHander(Character self, Character source, Vector2 Power);
public delegate void OnBuffAddedHandler(Character self, Character source, Buff buff, ref bool addBuff);
public delegate void OnPlayerActionHandler(PlayerBehaviour self, PlayerBehaviour.INPUT_FLAG iNPUT_FLAG);
public delegate void OnShootHandler(Character self, Projectile proj);



[Serializable]
public struct CharactorStatus
{
    public float maxHealth;
    public float moveSpeed;
    public float strength;
    public float recovery;
    public float knockback;

    public CharactorStatus(float maxHealth, float moveSpeed, float strength, float recovery, float knockback)
    {
        this.maxHealth = maxHealth;
        this.moveSpeed = moveSpeed;
        this.strength = strength;
        this.recovery = recovery;
        this.knockback = knockback;
    }

    public static CharactorStatus operator +(CharactorStatus a, CharactorStatus b) => new CharactorStatus(
        a.maxHealth + b.maxHealth,
        a.moveSpeed + b.moveSpeed,
        a.strength + b.strength,
        a.recovery + b.recovery,
        a.knockback + b.knockback);
    public static CharactorStatus operator *(CharactorStatus a, CharactorStatus b) => new CharactorStatus(
        a.maxHealth * b.maxHealth,
        a.moveSpeed * b.moveSpeed,
        a.strength * b.strength,
        a.recovery * b.recovery,
        a.knockback * b.knockback);

    public static CharactorStatus DefaultAdd => new CharactorStatus(0, 0, 0, 0, 0);
    public static CharactorStatus DefaultMulti => new CharactorStatus(1, 1, 1, 1, 1);
}

[Serializable]
public struct ProjectileInfo
{
    
    public Character source;
    public Character target;

    public float duration;
    public float damage;
    public float speedMultiplier;
    public bool ignorePlatform;
    public bool ignoreInv;
    public bool allowMultipleHit;
    public int fierceCount;
    public LayerMask targetLayerMask;

    public ProjectileInfo(Character source, Character target, float duration, float damage, float speedMultiplier, bool ignorePlatform, bool ignoreInv, bool allowMultipleHit, int fierceCount, LayerMask targetLayerMask)
    {
        this.source = source;
        this.target = target;
        this.duration = duration;
        this.damage = damage;
        this.speedMultiplier = speedMultiplier;
        this.ignorePlatform = ignorePlatform;
        this.ignoreInv = ignoreInv;
        this.allowMultipleHit = allowMultipleHit;
        this.fierceCount = fierceCount;
        this.targetLayerMask = targetLayerMask;
    }
}
public struct BuffInfo
{
    public string name;
    public Character target;
    public Character source;
    public float duration;
    public bool visibleInGui;

    public BuffInfo(string name, Character target, Character source, float duration, bool visibleInGui)
    {
        this.name = name;
        this.target = target;
        this.source = source;
        this.duration = duration;
        this.visibleInGui = visibleInGui;
    }
}

public struct CameraSetting
{
    public float topLimit, bottomLimit, leftLimit, rightLimit;
}

public struct CameraFollow
{
    public Transform follow;
    public Vector2 strength;

    public CameraFollow(Transform follow, Vector2 strength)
    {
        this.follow = follow;
        this.strength = strength;
    }
}