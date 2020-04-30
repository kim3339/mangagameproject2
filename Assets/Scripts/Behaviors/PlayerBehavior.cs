using UnityEngine;
using System.Collections;
using System;


public abstract class PlayerBehavior : Behavior
{
    public OnPlayerActionHandler OnPlayerActionHandler;

    public bool IsDodging => isDodging;
    public bool CanDodge => canDodge;
    public float LastLookDir => gameObject.transform.localScale.x;
    public bool CanUseUlt => canUseUlt;
    [SerializeField]
    float InitJumpHeight = 3f;

    float jumpPower;
    bool isDodging = false;
    bool canDodge = true;
    bool canUseUlt = true;

    Character player;
    Vector2 arrowInput;


    protected Control control;
    protected Animator animator;

    INPUT_FLAG input;
    INPUT_FLAG buffer;
    /// <summary>
	/// 입력에 대한 비트플래그
	/// </summary>
    public enum INPUT_FLAG
    {
        NONE = 0,
        JUMP = 1,
        DODGE = 1 << 1,
        FIRST = 1 << 2,
        SECOND = 1 << 3,
        THIRD = 1 << 4,
        FOURTH = 1 << 5,
        ULTIMATE = 1 << 6,
        CONTROL = 1 << 7,
        ALL = int.MaxValue
    }

    public override void Connect(Character self)
    {
        player = self;
    }
    public void OnEnable()
    {
        control.Enable();
    }

    public void OnDisable()
    {
        control.Disable();
    }

    void Awake()
    {
        jumpPower = Mathf.Sqrt(-2f * InitJumpHeight * Character.GRAVITY); ;
        animator = GetComponent<Animator>();
        control = new Control();
        //입력 핸들러 바인딩
        control.Player.Jump.started += (ctx) => buffer |= INPUT_FLAG.JUMP;
        control.Player.Dodge.started += (ctx) => buffer |= INPUT_FLAG.DODGE;
        control.Player.Skill1.started += (ctx) => buffer |= INPUT_FLAG.FIRST;
        control.Player.Skill2.started += (ctx) => buffer |= INPUT_FLAG.SECOND;
        control.Player.Skill3.started += (ctx) => buffer |= INPUT_FLAG.THIRD;
        control.Player.Skill4.started += (ctx) => buffer |= INPUT_FLAG.FOURTH;
        control.Player.Ultimate.started += (ctx) => buffer |= INPUT_FLAG.ULTIMATE;
        control.Player.Control.started += (ctx) => buffer |= INPUT_FLAG.CONTROL;
    }

    void Update()
    {
        // 새로 키가 입력되었을 경우 입력 갱신
        if(buffer != INPUT_FLAG.NONE)
        {
            input = buffer;
        }
        buffer = INPUT_FLAG.NONE;

        arrowInput = control.Player.Move.ReadValue<Vector2>();
        // 여기서 입력값이 받아진다.

        //이동
        float move = 0;

        //구르기 시 자동이동
        if (IsDodging)
            move = transform.localScale.x > 0 ? 1 : -1;
        else if(arrowInput.x != 0)
            move = arrowInput.x > 0 ? 1 : -1;

        player.Move(move);
        var _in = input;
        if (((input & INPUT_FLAG.JUMP) != 0) && player.IsGrounded) Jump();
        if ((input & INPUT_FLAG.DODGE) != 0 && CanDodge) Dodge();
        
        //구르는 중이 아니면 기술 사용, 사용 성공 시 해당 입력 플래그 제거
        if (IsDodging)
        {
            if ((input & INPUT_FLAG.FIRST) != 0) { if(First())
                {
                    input &= ~INPUT_FLAG.FIRST;
                }
            }
            if ((input & INPUT_FLAG.SECOND) != 0) { if (Second())
                {
                    input &= ~INPUT_FLAG.SECOND;
                }
            }
            if ((input & INPUT_FLAG.THIRD) != 0) { if (Third())
                {
                    input &= ~INPUT_FLAG.THIRD;
                }
            }
            if ((input & INPUT_FLAG.FOURTH) != 0) { if (Fourth())
                {
                    input &= ~INPUT_FLAG.FOURTH;
                }
            }
            if ((input & INPUT_FLAG.ULTIMATE) != 0 && CanUseUlt) { if (Ultimate())
                {
                    input &= ~INPUT_FLAG.ULTIMATE;
                }
            }
            if ((input & INPUT_FLAG.CONTROL) != 0) { if (Control())
                {
                    input &= ~INPUT_FLAG.CONTROL;
                }
            }
        }
        //입력 플래그가 하나라도 제거(사용)되었을시 선행입력 삭제
        if (input != _in) input = INPUT_FLAG.NONE;
        //점프/구르기는 선행입력 불가
        input &= ~(INPUT_FLAG.JUMP | INPUT_FLAG.DODGE);

    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        player.velocity.y += jumpPower;
    }

    void Dodge()
    {
        canDodge = false;
        isDodging = true;
        animator.SetTrigger("Dodge");
    }

    void DodgeEnd()
    {
        if (!isDodging) return;
        isDodging = false;
        StartCoroutine(DodgeDelay());
    }

    private IEnumerator DodgeDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canDodge = true;
    }
    

    /// <summary>
	/// 성장 시 실행되는 함수
	/// </summary>
    public abstract void LevelUp();
    /// <summary>
	/// 첫번째 스킬 사용시 실행되는 함수
	/// </summary>
    public abstract bool First();
    /// <summary>
	/// 두번째 스킬 사용시 실행되는 함수
	/// </summary>
    public abstract bool Second();
    /// <summary>
	/// 세번째 스킬 사용시 실행되는 함수
	/// </summary>
    public abstract bool Third();
    /// <summary>
	/// 네번째 스킬 사용시 실행되는 함수
	/// </summary>
    public abstract bool Fourth();
    /// <summary>
	/// 궁극기 사용시 실행되는 함수
	/// </summary>
    public abstract bool Ultimate();
    /// <summary>
	/// 범용 버튼 입력시 실행되는 함수
	/// </summary>
    public virtual bool Control() { return true; }
}
