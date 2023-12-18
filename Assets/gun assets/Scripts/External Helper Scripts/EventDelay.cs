using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDelay : MonoBehaviour
{
    [SerializeField] float delaySeconds = 1.0f;
    [SerializeField]UnityEvent onDelayComplete;
    [SerializeField] bool  doEventOnStart;

    private void Start()
    {
        if (!doEventOnStart) return;
        playEventAfterdelay();
    }

    public void playEventAfterdelay()
    {
        Invoke("PlayDelayCompleteEvent", delaySeconds);
    }
    private void PlayDelayCompleteEvent()
    {
        onDelayComplete.Invoke();
    }
}
