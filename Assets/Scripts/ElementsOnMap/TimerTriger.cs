using System.Collections.Generic;
using UnityEngine;

public class TimerTriger : MonoBehaviour, IInteractibleObiects
{
    [SerializeField] private float timeToActivateInSeconds = 5.0f;
    [SerializeField] private bool activateAfterTimeFinish = true;
    [SerializeField] private bool needPowerAllTime = true;
    [SerializeField] private List<GameObject> targetObjects;
    [SerializeField] private GameObject chargeIndicator;
    private bool isToggle;
    private float time = 0f;
    private bool isActivated = false;
    private Vector3 localScale;

    void Start()
    {
        localScale = chargeIndicator.transform.localScale;
        chargeIndicator.transform.localScale = new Vector3(localScale.x, 0f, localScale.z);
        if (!activateAfterTimeFinish)
            time = timeToActivateInSeconds;
    }

    private int tick = 0;
    void Update()
    {
        tick++;
        if (!isActivated || tick < 5) return;


        time = activateAfterTimeFinish ? time + Time.deltaTime * 5 : time - Time.deltaTime * 5;

        if (activateAfterTimeFinish && time >= timeToActivateInSeconds)
        {
            isActivated = false;
            time = timeToActivateInSeconds;
            foreach (GameObject obj in targetObjects)
            {
                if (isToggle)
                    obj.GetComponent<IInteractibleObiects>()?.ObiectToggle();
                else
                    obj.GetComponent<IInteractibleObiects>()?.ObiectInteract();
            }
        }
        else if (!activateAfterTimeFinish && time <= 0f)
        {
            isActivated = false;
            time = 0f;
            foreach (GameObject obj in targetObjects)
                obj.GetComponent<IInteractibleObiects>()?.ObiectRestart();
        }

        chargeIndicator.transform.localScale = new Vector3(localScale.x, time / timeToActivateInSeconds, localScale.z);
        if (!isActivated && !activateAfterTimeFinish) time = timeToActivateInSeconds;
        tick = 0;
    }

    private void OnInteraction(bool _isToggle)
    {
        isToggle = _isToggle;
        isActivated = true;
        if (activateAfterTimeFinish) return;

        foreach (GameObject obj in targetObjects)
        {
            if (isToggle)
                obj.GetComponent<IInteractibleObiects>()?.ObiectToggle();
            else
                obj.GetComponent<IInteractibleObiects>()?.ObiectInteract();
        }
    }

    public void ObiectInteract() => OnInteraction(false);
    public void ObiectToggle() => OnInteraction(true);
    public void ObiectRestart()
    {
        if (isToggle || (!needPowerAllTime && (time != 0f || time != timeToActivateInSeconds))) return;

        isActivated = false;
        time = activateAfterTimeFinish ? 0f : timeToActivateInSeconds;
        chargeIndicator.transform.localScale = new Vector3(localScale.x, 0f, localScale.z);
        foreach (GameObject obj in targetObjects)
            obj.GetComponent<IInteractibleObiects>()?.ObiectRestart();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new(1f, 1f, 0.3f, 0.4f);
        foreach (GameObject _object in targetObjects)
        {
            if (_object != null)
                Gizmos.DrawLine(transform.position, _object.transform.position);
        }
            
    }
}
