using UnityEngine;

public class MeteorBehavior : MonoBehaviour
{
    private Animator animator;
    private string[] animList = { "Anim1", "Anim2", "Anim3", "Anim4" };

    void Start()
    {
        animator = GetComponent<Animator>();
        int randomIndex = Random.Range(0, animList.Length);
        animator.SetBool(animList[randomIndex], true);
    }
}
