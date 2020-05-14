using UnityEngine;
using System.Collections;
using System;

public class Buff
{
    public BuffInfo info;
    float lifetime;
    Func<Character, Character, float,bool> condition;
    public void Init(Character target)
    {
        this.info.target = target;
        Enter();
    }
    public bool DurationCheck() 
    {
        Tick();
        lifetime += Time.deltaTime;

        //조건을 만족하거나 지속시간이 끝나면 종료
        var _cond = condition == null ? false : condition(info.target, info.source, lifetime);

        return _cond || (info.duration >= 0 ? lifetime > info.duration : false);

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
