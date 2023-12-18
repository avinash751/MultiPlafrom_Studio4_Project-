using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AmmoText : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI ammoText;

    private void OnEnable()
    {
        GunClass.currentGunAmmoHasBeenChanged += UpdateAmmoText;
    }

    private void OnDisable()
    {
        GunClass.currentGunAmmoHasBeenChanged -= UpdateAmmoText;
    }

    private void UpdateAmmoText(int ammoCount)
    {
       ammoText.text = ammoCount.ToString();
    }

    private void Start()
    {
        ammoText = GetComponent<TextMeshProUGUI>();
    }
}
