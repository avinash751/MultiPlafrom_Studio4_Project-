using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]Volume postProcessing;

    [SerializeField] Toggle postProcessingToggle;

    [SerializeField] Toggle meduimSettingsToggle;



    private void Start()
    {
        postProcessingToggle.isOn = postProcessing.enabled;
        meduimSettingsToggle.isOn = QualitySettings.GetQualityLevel()==2;
    }


    public void SetPostProcessing()
    {
        if(postProcessingToggle.isOn)
        {
            postProcessing.enabled = true;
        }
        else
        {
            postProcessing.enabled = false;
        }
    }


    public void SetGraphicQualitySettings(int _qualityLevel)
    {
        QualitySettings.SetQualityLevel(_qualityLevel);
    }
}
