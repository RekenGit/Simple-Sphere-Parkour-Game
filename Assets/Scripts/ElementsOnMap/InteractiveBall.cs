using UnityEngine;

public class InteractiveBall : MonoBehaviour, IInteractibleObiects
{
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private float respawnYThreshold;
    private void Awake()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    private void Start()
    {
        respawnYThreshold = GameManager.Instance.GetVelocityToRespawn();
    }

    public void ObiectInteract() 
    {
        transform.position = spawnPos;
        transform.rotation = spawnRot;
    }

    public void ObiectToggle() { }

    public void ObiectRestart() { }

    private int tick = 0;
    void Update()
    {
        tick++;
        if (tick < 60) return;

        if (transform.position.y < respawnYThreshold)
        {
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }
        tick = 0;
    }
}
