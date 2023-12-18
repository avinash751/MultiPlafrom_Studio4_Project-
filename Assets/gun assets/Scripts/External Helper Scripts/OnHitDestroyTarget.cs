using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnHitDestroyTarget : OnHitDamagable
{
   

    protected override void DoSomethingWhenObjectIsHit()
    {
        TargetsHitt.targetsHit++;
    }
}
