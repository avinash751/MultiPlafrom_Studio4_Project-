using System;
using UnityEngine;
using UnityEngine.Events;

public class OnHitDamagable : MonoBehaviour, IDamagable
{
    [SerializeField] private bool destroyOnDamage;
    [SerializeField] protected private UnityEvent onTakeDamage;
    

    public void TakeDamage(int damage)
    {
        onTakeDamage?.Invoke();
        DoSomethingWhenObjectIsHit();
        DestroyObject();
    }

    public  void DestroyObject()
    {
        if (destroyOnDamage)
        {
            Destroy(gameObject);
        }
       
    }
    protected virtual void DoSomethingWhenObjectIsHit()
    {

    }
}

