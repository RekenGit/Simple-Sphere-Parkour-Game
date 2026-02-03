using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject particlesGreen;
    [SerializeField] private GameObject particlesBlue;
    [SerializeField] private Material greenShader;
    [SerializeField] private Material blueShader;
    [SerializeField] private bool isLastCheckPoint = false;
    private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (isLastCheckPoint) return;
        GameManager.Instance.SetNewCheckPoint(this.gameObject);
        SetActiveCheckPoint(true);
        isLastCheckPoint = true;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private int tick = 0;
    private void Update()
    {
        tick++;
        if (tick < 5) return;

        if (GameManager.Instance.GetLastCheckPoint() != this.gameObject && isLastCheckPoint) 
            SetActiveCheckPoint(false);
        tick = 0;
    }

    public void SetActiveCheckPoint(bool isActive)
    {
        if (!isActive)
        {
            particlesBlue.SetActive(false);
            particlesGreen.SetActive(true);
            GetComponent<MeshRenderer>().material = greenShader;
            isLastCheckPoint = false;
        }
        else
        {
            audioSource.Play();
            particlesBlue.SetActive(true);
            particlesGreen.SetActive(false);
            GetComponent<MeshRenderer>().material = blueShader;
        }
    }
}
