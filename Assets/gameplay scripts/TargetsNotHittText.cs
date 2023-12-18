using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TargetsNotHittText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetsNotHitText;
    private void OnEnable()
    {
        TargetsNotHittDetector.TargetNotHitFound += UpdateTargetNotHittText;
        UpdateTargetNotHittText(5);
    }

    private void UpdateTargetNotHittText(int count)
    {
        targetsNotHitText.text = "X " +count.ToString();
    }

    private void OnDisable()
    {
        TargetsNotHittDetector.TargetNotHitFound -= UpdateTargetNotHittText;
    }
    void Start()
    {
        targetsNotHitText = GetComponent<TextMeshProUGUI>();
    }


}
