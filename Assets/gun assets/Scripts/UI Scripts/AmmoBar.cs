using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AmmoBar : VisulaBar
{
    private void OnEnable()
    {
        GunClass.BulletHasBeenShot+=SetNewCurrentValueAndMaxValueAndUpdateBar;
    }

    private void OnDisable()
    {
        GunClass.BulletHasBeenShot -= SetNewCurrentValueAndMaxValueAndUpdateBar;
    }

}
