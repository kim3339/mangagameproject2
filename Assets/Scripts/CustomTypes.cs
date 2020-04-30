using UnityEngine;

public delegate void OnHealthChangedHandler(Character self, Character source, float amount);
public delegate void OnDeadHandler(Character self, string cause, bool destroyObject);
public delegate void OnKnockbackedHander(Character self, Character source, Vector2 Power);
public delegate void OnBuffAddedHandler(Character self, Character source, Buff buff, ref bool addBuff);
public delegate void OnPlayerActionHandler(PlayerBehavior self, PlayerBehavior.INPUT_FLAG iNPUT_FLAG);
public delegate void OnShootHandler(Character self, ref ProjectileInfo info);

[System.Serializable]
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

[System.Serializable]
public struct ProjectileInfo
{
    public Character source;
    public Character target;

    public float duration;
    public float damage;
    public bool ignorePlatform;
    public int fierceCount;
    public int targetLayerMask;
}