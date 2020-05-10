using UnityEngine;
using System.Collections;

public class TestPlayerBehaviour : PlayerBehaviour
{
    public GameObject testProj;

    public override bool First()
    {
        Debug.Log(self.CreateProjectile(testProj,transform.position,transform.right,0));
        return true;
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
}
