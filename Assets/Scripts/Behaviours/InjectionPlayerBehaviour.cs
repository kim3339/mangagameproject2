using UnityEngine;
using System.Collections;

public class InjectionPlayerBehaviour : PlayerBehaviour
{
    public GameObject skill1Proj;
    bool canUseSkill = true;
    bool canUseSkill1 = true;
    bool attack = false;
    public override bool First()
    {
        if(!canUseSkill1) return false;
        canUseSkill1 = false;
        canUseSkill = false;
        animator.SetBool("inj_skill1_type",attack);
        animator.SetTrigger("inj_skill1");
        attack = !attack;
        if(control.Player.Move.ReadValue<Vector2>().x != 0)
        transform.localScale = new Vector3(Mathf.Sign(control.Player.Move.ReadValue<Vector2>().x),1,1);
        return true;
    }
    private void first()  => StartCoroutine(_first());
    private IEnumerator _first()
    {
        var face = Mathf.Sign (transform.localScale.x);
        var wait = new WaitForSeconds(0.1f);
        for(int i = 0; i < 3; i++)
        {
            if(IsDodging) break;
            var ins = self.CreateProjectile(skill1Proj, transform.position + face * new Vector3(Random.Range(0.9f,1.2f),Random.Range(-0.2f,0.2f), 0), 1 << 11);
            ins.OnUpdate += (Projectile p) => p.Move((8 * Time.deltaTime +  3 * p.LifeTime * Time.deltaTime) * Vector3.right * face );
            float angle = Mathf.Atan2(0, self.controller.Collisions.faceDir) * Mathf.Rad2Deg;
            ins.transform.rotation = Quaternion.Euler(0,0,angle);
            yield return wait;
        }
    }
    private void Skill1Done() => canUseSkill1 = true;
    private void SkillDone()
    {
        if(canUseSkill1) canUseSkill = true;
    } 
    public override bool Fourth()
    {
        throw new System.NotImplementedException();
    }

    public override void LevelUp()
    {
        throw new System.NotImplementedException();
    }

    public override bool Second()
    {
        throw new System.NotImplementedException();
    }

    public override bool Third()
    {
        throw new System.NotImplementedException();
    }

    public override bool Ultimate()
    {
        throw new System.NotImplementedException();
    }

    protected override void Move(float move)
    {
        if(canUseSkill) 
        {
            self.Move(move);
            if(move != 0) attack = false;
        }
    }

    protected override void Dodge()
    {
        base.Dodge();
        canUseSkill = true;
        canUseSkill1 = true;
        attack = false;
    }

    protected override void Jump()
    {
        if(canUseSkill) base.Jump();
        attack = false;
    }
}
