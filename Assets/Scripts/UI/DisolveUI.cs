using System.Collections;
using UnityEngine;

public class DisolveUI : MonoBehaviour
{
    [SerializeField] private float disolveDurationTime;
    [SerializeField] private Material UIDisolveObject;
    private bool isDisolveVisible = true;
    private AudioSource audioSource;

    void Start()
    {
        //UIDisolveObject = GetComponent<Renderer>().material;
        PlayerCustomizationObject customizations = GameManager.Instance.GetPlayerCustomization();
        UIDisolveObject.SetColor("_EmissionColor", customizations.glowColors[customizations.selectedGlowColorId]);
        UIDisolveObject.SetFloat("_TreshHold", 0);
        GetComponent<Renderer>().material = UIDisolveObject;
        audioSource = GetComponent<AudioSource>();
        DisolveUIToggle();
    }
    public void DisolveUIToggle() => StartCoroutine(WaitUntilCoroutineFinish());
    private IEnumerator WaitUntilCoroutineFinish()
    {
        audioSource.Play();
        StopAllCoroutines();
        isDisolveVisible = !isDisolveVisible;
        yield return StartCoroutine(StartUIDisolve());
    }
    private IEnumerator StartUIDisolve()
    {
        float time = 0;
        float strenght;
        float _from = UIDisolveObject.GetFloat("_TreshHold");
        float _to = isDisolveVisible ? 0f : 1f;
        while (time < disolveDurationTime)
        {
            time += Time.deltaTime;
            strenght = Mathf.Lerp(_from, _to, time / disolveDurationTime);
            UIDisolveObject.SetFloat("_TreshHold", strenght);
            yield return null;
        }
    }
}
