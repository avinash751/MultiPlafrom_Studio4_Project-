using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    public abstract void TakeDamage(int Amount);

    public void DestroyObject();


}
