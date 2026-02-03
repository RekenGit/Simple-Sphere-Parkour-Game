using System.Collections;
using UnityEngine;

public class FloodFloor : MonoBehaviour, IInteractibleObiects
{
    [SerializeField] [Range(0.1f, 120f)] private float durationFlood = 10f;
    [SerializeField] [Range(0.1f, 120f)] private float durationDrouth = 0.2f;
    [SerializeField] [Range(0.1f, 120f)] private float waitTimeBeforeFlood = 0.1f;
    [SerializeField] private float floodSize = 5f;
    [SerializeField] private bool StartOnTrigger = true;
    private float startSize;
    private bool isFlooding = true;
    private bool isTriggered = false;

    void Start()
    {
        startSize = transform.localScale.y;
        InvokeRepeating("ChangeObjectDestination", 0, 0.1f);
        isTriggered = !StartOnTrigger;
        if (isTriggered)
            StartCoroutine(ModifySize(startSize, floodSize, durationFlood, waitTimeBeforeFlood));
    }

    private void ChangeObjectDestination()
    {
        if (!isTriggered) return;

        Vector3 targetPosition = isFlooding ? new Vector3(transform.localScale.x, floodSize, transform.localScale.z) : new Vector3(transform.localScale.x, startSize, transform.localScale.z);
        if (Vector3.Distance(transform.localScale, targetPosition) == 0)
        {
            if (isFlooding)
            {
                isFlooding = false;
                StartCoroutine(ModifySize(floodSize, startSize, durationDrouth));
            }
            else
            {
                isFlooding = true;
                StartCoroutine(ModifySize(startSize, floodSize, durationFlood, waitTimeBeforeFlood));
            }
        }
    }

    private IEnumerator ModifySize(float startSize, float endSize, float durationTime, float waitTime = 0.1f)
    {
        if (waitTime > 0) 
            yield return new WaitForSeconds(waitTime);
        float time = 0;
        float  sizeLength;

        while (time < durationTime)
        {
            time += Time.deltaTime;
            sizeLength = Mathf.Lerp(startSize, endSize, time / durationTime);
            transform.localScale = new Vector3(transform.localScale.x, sizeLength, transform.localScale.z);

            yield return null;
        }
    }

    public void ObiectInteract()
    {
        isTriggered = true;
        StartCoroutine(ModifySize(startSize, floodSize, durationFlood, waitTimeBeforeFlood));
    }

    public void ObiectToggle() => ObiectInteract();

    public void ObiectRestart()
    {
        StopAllCoroutines();
        isFlooding = true;
        StartCoroutine(ModifySize(transform.localScale.y, startSize, 3f));
        isTriggered = !StartOnTrigger;
        if (isTriggered)
            StartCoroutine(ModifySize(startSize, floodSize, durationFlood, waitTimeBeforeFlood));
    }
}
