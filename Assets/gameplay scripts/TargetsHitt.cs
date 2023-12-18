using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetsHitt : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI text;

    public static int targetsHit = 0;

    private void OnEnable()
    {
        GameManager.GameIsStarting += ResetAmmo;
        ResetAmmo();
    }

    private void OnDisable()
    {
        GameManager.GameIsStarting -= ResetAmmo;
    }

    private void Update()
    {
        text.text = "TARGETS " + targetsHit;
    }

    private void ResetAmmo()
    {
        targetsHit = 0;
        text.text = "TARGETS " + targetsHit;
    }
    

}
