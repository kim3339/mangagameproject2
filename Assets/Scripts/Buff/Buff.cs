using UnityEngine;
using System.Collections;
using System;

public class Buff
{
    public readonly string name = "이름 없는 버프";
    public Character Owner => source;
    float duration;
    float lifetime;
    Character source;
    Character target;
    Func<Character, Character, float,bool> condition;
    public bool isVisible;

    public Buff(string name,
            float duration = float.PositiveInfinity,
            Character source = null, 
            Func<Character, Character,float,bool> condition = null,
            bool isVisible = false)
    {
        this.name = source == null ? "" : (source.Name + "의 ") + name;
        this.condition = condition;
        this.duration = duration;
        this.isVisible = isVisible;
        lifetime = 0;
    }
    public void Init(Character target)
    {
        this.target = target;
        Enter();
    }
    public bool DurationCheck() 
    {
        Tick();
        lifetime += Time.deltaTime;

        //조건을 만족하거나 지속시간이 끝나면 종료
        var _cond = condition == null ? false : condition(source, target, lifetime);

        return _cond || lifetime > duration;

    }
    /// <summary>
	/// 버프 시작 시 실행되는 함수
	/// </summary>
    protected virtual void Enter() { }
    /// <summary>
	/// 버프 지속시간동안 실행되는 함수
	/// </summary>
    protected virtual void Tick() { }
    /// <summary>
	/// 버프 끝날 때 실행되는 함수
	/// </summary>
    public virtual void Exit() { }
}
