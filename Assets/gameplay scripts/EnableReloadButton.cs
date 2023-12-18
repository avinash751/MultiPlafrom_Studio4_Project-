using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableReloadButton : MonoBehaviour
{

    ReloadButton reloadButton;
    [SerializeField] float disbaleTimer;

    void Start()
    {
        reloadButton = FindObjectOfType<ReloadButton>(true);
    }

    public void EnableThisReloadButton()
    {
        reloadButton.gameObject.SetActive(true);
        Invoke("DisableThisReloadButton", disbaleTimer);
    }

    public void DisableThisReloadButton()
    {
        reloadButton.gameObject.SetActive(false);
    }
}
