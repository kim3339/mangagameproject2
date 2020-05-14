using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController2D),typeof(Animator))]
public class Character : MonoBehaviour
{
    public static bool IsInteracting = false;
    // 이벤트
    /// <summary>
	/// 데미지를 받았을 때 발생하는 이벤트입니다.
	/// </summary>
    public OnHealthChangedHandler OnDamaged;
    /// <summary>
	/// 회복을 받았을 때 발생하는 이벤트입니다.
	/// </summary>
    public OnHealthChangedHandler OnHealed;
    /// <summary>
	/// 죽었을 때 발생하는 이벤트입니다.
	/// </summary>
    public OnDeadHandler OnDead;
    /// <summary>
	/// 넉백을 받았을 때 발생하는 이벤트입니다.
	/// </summary>
    public OnKnockbackedHander OnKnockbacked;
    /// <summary>
	/// 버프가 추가되었을 때 발생하는 이벤트입니다.
	/// </summary>
    public OnBuffAddedHandler OnBuffAdded;
    public OnShootHandler OnShoot;

    public Func<float> OnDamagedAdder;
    public Func<float> OnDamagedMultiplier;

    public Func<float> OnHealedAdder;
    public Func<float> OnHealedMultiplier;
    // 기본 능력치
    /// <summary>
	/// 체력
	/// </summary>
    public float Health { get => health; }
    public CharactorStatus Status => calculatedStatus;
    public bool IsGrounded => controller.Collisions.below;
    public int CanMove { get => canFixedUpdate; set => canFixedUpdate = Mathf.Max(0, value); }
    public int CanAct { get => canUpdate; set => canUpdate = Mathf.Max(0, value); }
    public int Invincibility { get => invincibility; set => invincibility = Mathf.Max(0, value); }

    public string Name { get => name; }

    [HideInInspector]
    public Vector3 velocity;

    // 컴포넌트
    
    [SerializeField]
    private Behaviour behaviour;
    [HideInInspector]
    public new Transform transform;
    [HideInInspector]
    public CharacterController2D controller;
    [HideInInspector]
    public Animator animator;
    public BoxCollider2D collider => controller.collider;

    SpriteRenderer renderer;

    //내부 파라미터
    [SerializeField]
    CharactorStatus initStatus = new CharactorStatus(100, 3, 3, 3, 1);
    CharactorStatus calculatedStatus;

    int canUpdate = 0;
    int canFixedUpdate = 0;

    public const float GRAVITY = -20;
    const float AIRDAMP = 0.01f;
    const float GROUNDDAMP = 0.03f;
    float health;
    Vector3 moveVelocity = Vector3.zero;
    List<Buff> buffs = new List<Buff>();
    bool isBuffUpdating = false;
    
    bool hasGravity = true;
    [SerializeField] new private string name = "이름 없음";
    int invincibility = 0;

    float hitEffectAlpha = 0;

    // Use this for initialization
    void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        transform = gameObject.transform;
        velocity = Vector3.zero;

        

        calculatedStatus = initStatus;
        health = calculatedStatus.maxHealth;

        buffs.Clear();
        StartCoroutine(HpRegenerate());

        if (behaviour != null)
        {
            behaviour.Connect(this);
            StartCoroutine(BehaviourManager());
        }
    }

    IEnumerator BehaviourManager()
    {
        var waitForDisable = new WaitWhile(() => CanAct == 0 || IsInteracting);
        var waitForEnable = new WaitUntil(() => CanAct == 0 || IsInteracting);

        while (true)
        {
            yield return waitForDisable;
            behaviour.enabled = false;
            yield return waitForEnable;
            behaviour.enabled = true;
        }
    }

    void Update()
    {
        if(hitEffectAlpha > 0)
        {
            renderer.color = new Color(1,1 - hitEffectAlpha, 1 - hitEffectAlpha);
            hitEffectAlpha -= Time.deltaTime * 3;
        }
        else
        {
            renderer.color = Color.white;
            hitEffectAlpha = 0;
        }
    }

    void FixedUpdate()
    {
        if(CanMove != 0)
        {
            velocity = Vector3.zero;
            return;
        }


        velocity.x = (IsGrounded) ? velocity.x * (1 - GROUNDDAMP) : velocity.x * (1 - AIRDAMP);

        if ((controller.Collisions.above && velocity.y > 0) || (controller.Collisions.below && velocity.y < 0)) velocity.y = 0;

        
        // 중력
        if (hasGravity)
            velocity.y += GRAVITY * Time.fixedDeltaTime;
       
        animator.SetBool("IsFalling", IsGrounded ? false : velocity.y < 0);

        controller.Move((velocity + moveVelocity) * Time.fixedDeltaTime);

        animator.SetBool("IsGrounded", IsGrounded);

        moveVelocity = Vector3.zero;
    }

    private IEnumerator HpRegenerate()
    {
        var waitaSecond = new WaitForSeconds(1);
        while(true)
        {
            if (Health != Status.maxHealth)
            {
                Heal(this, Status.recovery);
            }
            yield return waitaSecond;
        }
    }

    

    #region public 함수
    /// <summary>
	/// 데미지를 받습니다.
	/// </summary>
    public float Damage(Character source, float amount, bool ignoreInv = false, bool hitEffect = true)
    {
        if (Invincibility > 0 && !ignoreInv) return 0;
        float value = amount;

        if (OnDamagedMultiplier != null)
        {
            var list = OnDamagedMultiplier.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                value *= (float)list[i].DynamicInvoke();
            }
        }
        if (OnDamagedAdder != null)
        {
            var list = OnDamagedAdder.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                value += (float)list[i].DynamicInvoke();
            }
        }

        value = Mathf.Max(0, value);
        OnDamaged?.Invoke(this, source, value);

        if (value < 0) return 0;

        health = Mathf.Max(0, Health - value);
        if (health == 0) Dead(source?.Name);
        else if(hitEffect)
        {
            hitEffectAlpha = 0.6f;
        }
        return value;
    }
    /// <summary>
	/// 회복을 받습니다.
	/// </summary>
    public float Heal(Character source, float amount)
    {
        float value = amount;

        if (OnHealedMultiplier != null)
        {
            var list = OnHealedMultiplier.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                value *= (float)list[i].DynamicInvoke();
            }
        }
        if (OnHealedAdder != null)
        {
            var list = OnHealedAdder.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                value += (float)list[i].DynamicInvoke();
            }
        }

        value = Mathf.Max(0, value);
        OnHealed?.Invoke(this, source, value);

        if (value < 0) return 0;

        health = Mathf.Min(Status.maxHealth , Health + value);
        if (health == 0)
        {
            Dead(source?.Name);
        }
        return value;
    }
    /// <summary>
	/// 죽습니다.
	/// </summary>
    public void Dead(string cause)
    {
        if (cause == null) cause = "자비없는 세계";
    
        
        bool destroyit = true;
        if(!destroyit) return;

        Destroy(gameObject);
        OnDead?.Invoke(this, cause, ref destroyit);
        Debug.Log(Name + "은(는) " + cause + "에 의해 죽었습니다.");
    }
    /// <summary>
	/// 넉백을 받습니다.
	/// </summary>
    public void Knockback(Character source,Vector2 power)
    {
        OnKnockbacked?.Invoke(this, source, power);
        velocity += (Vector3)power * Status.knockback;
        StartCoroutine(knockbackWhileGround());
    }

    private IEnumerator knockbackWhileGround()
    {
        CanAct++;
        animator.SetTrigger("OnKnockbacked");
        yield return new WaitUntil(() => IsGrounded);
        CanAct--;
    }

    public void Move(float move)
    {
        if (CanMove != 0)
        {
            moveVelocity = Vector3.zero;
            return;
        }

        moveVelocity = Vector2.right * move * Status.moveSpeed;
        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        animator.SetBool("Move", move != 0);

    }

    public void GoDownPlatform() => controller.TriggerGoDown();

    public Projectile CreateProjectile(GameObject prefab, Vector3 pos, Vector2 velocity, int targetLayerMask)
    {
        if (prefab.GetComponent<Projectile>() == null) return null;
        
        var ins = Instantiate(prefab).GetComponent<Projectile>();
        ins.transform.position = pos;
        ins.OnUpdate += (p) => p.Move(velocity * Time.deltaTime);
        ins.info.targetLayerMask = targetLayerMask;
        ins.info.source = this;

        OnShoot?.Invoke(this, ins);
        return ins;
    }
    public Projectile CreateProjectile(GameObject prefab, Vector3 pos, int targetLayerMask)
    {
        if (prefab.GetComponent<Projectile>() == null) return null;
        var ins = Instantiate(prefab).GetComponent<Projectile>();
        ins.transform.position = pos;
        ins.info.source = this;
        ins.info.targetLayerMask = targetLayerMask;
        OnShoot?.Invoke(this, ins);
        return ins;
    }

    #region Buff Management

    private IEnumerator BuffUpdate()
    {
        isBuffUpdating = true;
        while(buffs.Count > 0)
        {
            CharactorStatus _multi = CharactorStatus.DefaultMulti;
            CharactorStatus _add = CharactorStatus.DefaultAdd;
            buffs.RemoveAll((b) =>
            {
                bool exit = b.DurationCheck();
                if(b is IStatusBuff)
                {
                    _add += (b as IStatusBuff).AddAmount;
                    _multi *= (b as IStatusBuff).MultiAmount;
                }
                if (exit)
                    b.Exit();
                return exit;
            });
            calculatedStatus = (calculatedStatus * _multi) + _add;
            yield return null;
        }
        isBuffUpdating = false;
    }
    /// <summary>
	/// 버프를 추가합니다.
	/// </summary>
    public void AddBuff(Buff buff)
    {
        bool value = true;
        OnBuffAdded?.Invoke(this, buff.info.source, buff, ref value);
        if(value)
        {
        buffs.Add(buff);
        buff.Init(this);
        
        if (!isBuffUpdating)
            StartCoroutine(BuffUpdate());
        }
    }
    /// <summary>
	/// 특정 버프군을 전부 제거합니다.
	/// </summary>
    public void RemoveBuffAll<T>(bool executeExitMethod = true) where T : Buff
    {
        bool CompareIncludeExitMethod(Buff _buff)
        {
            _buff.Exit();
            return _buff is T;
        }
        if (executeExitMethod)
            buffs.RemoveAll((b) => CompareIncludeExitMethod(b));
        else
            buffs.RemoveAll((b) => b is T);
    }
    /// <summary>
	/// 버프를 모두 제거합니다.
	/// </summary>
    public void RemoveBuffAll(bool executeExitMethod = true)
    {
        if (executeExitMethod) buffs.ForEach((b) => b.Exit());
        buffs.Clear();
    }
    /// <summary>
	/// 특정 버프를 제거합니다.
	/// </summary>
    public void RemoveBuff(Buff buff, bool executeExitMethod = true)
    {
        if(buffs.Contains(buff))
        {
            if(executeExitMethod) buff.Exit();
            buffs.Remove(buff);
        }
    }
    #endregion

    #endregion

    

}
