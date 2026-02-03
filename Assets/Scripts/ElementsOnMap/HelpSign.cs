using System.Collections;
using TMPro;
using UnityEngine;

public class HelpSign : MonoBehaviour, IInteractibleObiects
{
    [SerializeField] private string text;
    [SerializeField] private string secondText;
    [SerializeField] private int fontSize;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private bool isActiveOnStart;
    private Transform playerCamera;
    private AudioSource audioSource;
    
    public void OnEditorChange()
    {
        textField.text = text;
        textField.fontSize = fontSize;
    }

    void Start()
    {
        textField.text = text;
        textField.fontSize = fontSize;
        audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(FindCamera());
    }

    private IEnumerator FindCamera()
    {
        yield return new WaitForSeconds(0.5f);
        playerCamera = GameManager.Instance.GetMainCamera().transform;
        if (!isActiveOnStart)
            gameObject.SetActive(false);
    }

    private int tick = 0;
    void Update()
    {
        tick++;
        if (playerCamera == null || tick < 5)
            return;
        
        transform.LookAt(playerCamera);
        tick = 0;
    }

    public void ObiectInteract() {
        if (string.IsNullOrEmpty(secondText) || !gameObject.activeSelf)
            return;

        StartCoroutine(ChangeText());
    }

    private IEnumerator ChangeText()
    {
        string currentText = textField.text;
        textField.text = "";
        yield return new WaitForSeconds(0.4f);
        audioSource.Play();
        if (currentText == text)
            textField.text = secondText;
        else
            textField.text = text;
    }

    public void ObiectToggle() => gameObject.SetActive(!gameObject.activeSelf);

    public void ObiectRestart()
    {
        textField.text = text;
        if (!isActiveOnStart)
            gameObject.SetActive(false);
    }
}
