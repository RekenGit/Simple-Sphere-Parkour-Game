using UnityEngine;

public class DoorOpenClose : MonoBehaviour, IInteractibleObiects
{
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private int doorsNeededToOpen = 1;
    private Animator anim;
    private AudioSource audioSource;
    private int doorsClicked = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ObiectInteract()
    {
        doorsClicked++;
        if (doorsClicked >= doorsNeededToOpen)
        {
            anim.SetBool("DoorOpen", true);
            audioSource.clip = openSound;
            audioSource.Play();
        }
    }

    public void ObiectToggle()
    {
        bool isOpen = anim.GetBool("DoorOpen");
        anim.SetBool("DoorOpen", !isOpen);
        audioSource.clip = isOpen ? openSound : closeSound;
        audioSource.Play();
    }

    public void ObiectRestart()
    {
        doorsClicked = 0;
        if (!anim.GetBool("DoorOpen")) return;

        anim.SetBool("DoorOpen", false);
        audioSource.clip = closeSound;
        audioSource.Play();
    }
}
