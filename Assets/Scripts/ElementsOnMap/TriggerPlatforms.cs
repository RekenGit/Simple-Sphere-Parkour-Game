using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerPlatforms : MonoBehaviour
{
    [SerializeField] public TriggerType triggerType;
    private PlayerMovement playerScript;
    private Material objectMaterial;
    private bool isActive = true;

    //Disolve
    [SerializeField] private AudioClip audioClip; //+Button
    [SerializeField] private float timeToDisapear = 1;
    [SerializeField] private float timeToApear = 4;
    [SerializeField] private GameObject objectToDisapear;

    //Button
    [SerializeField] private bool restartOnPlayerDeath;
    [SerializeField] private bool isToggle;
    [SerializeField] private bool canDeactivateOnExit;
    [SerializeField] private List<GameObject> targetObjects;
    private Animator animator;
    private AudioSource audioSource;

    public enum TriggerType
    {
        JumpBoost = 0,
        SpeedBoost,
        DisolveFloor,
        Button,
        LevelEnd,
    }

    private void OnDrawGizmos()
    {
        if (triggerType == TriggerType.Button)
        {
            Gizmos.color = new(0f, 1f, 1f, 0.4f);
            foreach (GameObject _object in targetObjects)
                Gizmos.DrawLine(transform.position, _object.transform.position);
        }
        else if (triggerType == TriggerType.JumpBoost)
        {
            Gizmos.color = new(1f, 1f, 0f, 0.2f);
            Gizmos.DrawSphere(transform.position, 15.5f);
        }
        else if (triggerType == TriggerType.SpeedBoost)
        {
            Gizmos.color = new(0f, 0.5f, 1f, 0.2f);
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, new Vector3(38.5f, 2.2f, 38.5f));
        }
    }

    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (objectToDisapear != null ) objectMaterial = objectToDisapear.GetComponent<Renderer>().material;
        if (triggerType == TriggerType.DisolveFloor || triggerType == TriggerType.Button || triggerType == TriggerType.LevelEnd)
        {
            if (audioClip != null)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.clip = audioClip;
            }

            if (triggerType == TriggerType.DisolveFloor) 
                isActive = false;
            else if (triggerType == TriggerType.Button)
            {
                playerScript.playerDied.AddListener(OnPlayerDeath);
                animator = GetComponent<Animator>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (triggerType)
        {
            case TriggerType.JumpBoost:
                playerScript.isOnJumpBoost = true;
                break;

            case TriggerType.SpeedBoost:
                playerScript.isOnSpeedBoost = true;
                break;

            case TriggerType.DisolveFloor:
                if (!isActive)
                {
                    if (audioClip != null)
                        audioSource.Play();
                    StartCoroutine(Dissolve());
                }
                break;

            case TriggerType.Button:
                if (animator != null)
                    animator.SetBool("IsPressed", true);
                else
                    GetComponent<Collider>().enabled = false;
                if (audioClip != null)
                    audioSource.Play();
                foreach (GameObject _object in targetObjects)
                {
                    if (isToggle) _object.GetComponent<IInteractibleObiects>()?.ObiectToggle();
                    else _object.GetComponent<IInteractibleObiects>()?.ObiectInteract();
                }
                break;

            case TriggerType.LevelEnd:
                FinishLevel();
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (triggerType)
        {
            case TriggerType.JumpBoost:
                playerScript.isOnJumpBoost = false;
                break;

            case TriggerType.SpeedBoost:
                playerScript.isOnSpeedBoost = false;
                break;

            case TriggerType.Button:
                if (canDeactivateOnExit)
                {
                    if (animator != null)
                        animator.SetBool("IsPressed", false);
                    else
                        GetComponent<Collider>().enabled = true;
                    foreach (GameObject _object in targetObjects)
                        _object.GetComponent<IInteractibleObiects>()?.ObiectRestart();
                }
                break;
        }
    }

    private void OnPlayerDeath()
    {
        if (!restartOnPlayerDeath) return;

        if (triggerType == TriggerType.Button)
        {
            if (animator != null)
                animator.SetBool("IsPressed", false);
            else
                GetComponent<Collider>().enabled = true;
            foreach (GameObject _object in targetObjects)
                _object.GetComponent<IInteractibleObiects>()?.ObiectRestart();
        }
    }

    #region Disolve
    private IEnumerator Dissolve()
    {
        isActive = true;
        float time = 0;
        float dissolveStrength;
        while (time < timeToDisapear)
        {
            time += Time.deltaTime;
            dissolveStrength = Mathf.Lerp(0, 1, time / timeToDisapear);
            objectMaterial.SetFloat("_TreshHold", dissolveStrength);
            if (objectMaterial.GetFloat("_TreshHold") > 0.7) StartCoroutine(DisapearAndApear());
            yield return null;
        }
    }

    private IEnumerator DisapearAndApear()
    {
        objectToDisapear.GetComponent<BoxCollider>().enabled = false;

        float time = 0;
        while (time < timeToApear)
        {
            time += Time.deltaTime;
            if (Mathf.Lerp(0, 1, time / timeToApear) > 0.8)
            {
                objectMaterial.SetFloat("_TreshHold", 0);
                objectToDisapear.GetComponent<BoxCollider>().enabled = true;
            }
            yield return null;
        }
        isActive = false;
    }
    #endregion
    #region LevelEnd
    private void FinishLevel()
    {
        audioSource.Play();
        int actualLevel = Int32.Parse(SceneManager.GetActiveScene().name.Replace("Level", ""));
        Debug.Log("Finished Level: " + actualLevel);
        if (PlayerPrefs.GetInt("LastSavedLevel") <= actualLevel)
            PlayerPrefs.SetInt("LastSavedLevel", actualLevel + 1);

        GameManager.Instance.FinishedLevelSuccessfully();
        GameManager.Instance.LoadLevel("Level" + (actualLevel + 1));
    }
    #endregion
}
