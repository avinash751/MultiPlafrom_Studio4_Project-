using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetsNotHittDetector : MonoBehaviour
{
    [SerializeField] private int TotalTargetsAllowedToBeNotShot;
    public static Action<int> TargetNotHitFound;


    private void Start()
    {
        TargetNotHitFound?.Invoke(TotalTargetsAllowedToBeNotShot);
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            TotalTargetsAllowedToBeNotShot--;
            TargetNotHitFound?.Invoke(TotalTargetsAllowedToBeNotShot);
            damagable.DestroyObject();

        }
    }

}
