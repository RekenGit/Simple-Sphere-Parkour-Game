using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float drag;
    [SerializeField] private float animationDuration;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask deathLayer;
    [Header("Objects")]
    [SerializeField] private GameObject attachmentPrefab;
    [SerializeField] private GameObject playerCamPrefab;
    [SerializeField] private Material sphereMaterial;
    [Header("Sounds")]
    [SerializeField] private AudioClip fallDownSound;
    [SerializeField] private AudioClip fallingInAirSound;
    [SerializeField] private AudioClip teleportSound;
    [SerializeField] private AudioClip lavaBurn;
    private PlayerControls playerInputActions;
    public UnityEvent playerDied;

    private Color glowColor;
    private Transform attachments;
    private Transform playerCam;
    private ParticleSystem groundParticles;
    private Rigidbody sphereRigidBody;
    private LayerMask iceLayer;
    private AudioSource audioSourceMain;
    private AudioSource audioSourceFalling;
    public float restartVelocity;
    public bool isOnJumpBoost = false;
    public bool isOnSpeedBoost = false;
    private bool isGrounded;
    private bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            if (value != isGrounded) 
            {
                isGrounded = value;

                if (isGrounded)
                {
                    Destroy(audioSourceFalling);

                    audioSourceMain.clip = fallDownSound;
                    float rand = Random.Range(0.4f, 0.6f);
                    audioSourceMain.volume = rand;
                    audioSourceMain.pitch = rand + 0.4f;
                    audioSourceMain.spatialBlend = 1f;
                    audioSourceMain.Play();
                }
                else
                {
                    audioSourceFalling = gameObject.AddComponent<AudioSource>();
                    audioSourceFalling.clip = fallingInAirSound;
                    audioSourceFalling.loop = true;
                    audioSourceFalling.volume = 0f;
                    audioSourceFalling.Play();
                }
            }
        }
    }
    private bool isOnIce;
    private const float speedinterval = 2f;
    private Vector3 playerMoveDirection;
    private bool isSpacePressed;

    public PlayerControls GetPlayerInputActions() => playerInputActions;

    private void Start()
    {
        iceLayer = LayerMask.GetMask("Ice");
        sphereRigidBody = GetComponent<Rigidbody>();
        audioSourceMain = GetComponent<AudioSource>();
        attachments = Instantiate(attachmentPrefab, transform.position, Quaternion.identity).transform;
        playerCam = Instantiate(playerCamPrefab, transform.position, Quaternion.identity).transform;

        playerInputActions = new PlayerControls();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += PlayerJump;
        playerInputActions.Player.RestartPlayer.performed += PlayerRestart;
        playerInputActions.Player.RestartLevel.performed += PlayerLevelRestart;
        //Player UI input is in CameraMovement script

        PlayerCustomizationObject customizations = GameManager.Instance.GetPlayerCustomization();
        GetComponent<MeshFilter>().mesh = customizations.sphereMeshes[customizations.selectedMeshId];
        sphereMaterial.SetColor("_Color", customizations.baseColors[customizations.selectedBaseColorId]);
        sphereMaterial.SetColor("_EmissionColor", customizations.glowColors[customizations.selectedGlowColorId]);

        groundParticles = attachments.Find("GroundParticles").GetComponent<ParticleSystem>();
        restartVelocity = GameManager.Instance.GetVelocityToRespawn();
        //sphereMaterial = GetComponent<Renderer>().material;
        //GetComponent<Renderer>().material = sphereLocalMaterial;
        glowColor = sphereMaterial.GetColor("_EmissionColor");

#if UNITY_ANDROID
        drag = drag * 2f;
        speed = speed * 1.5f;
        jumpHeight = jumpHeight * 1.1f;
#endif
    }

    void Update()
    {
        if (isKilled)
        {
            ChangePlayerMaterialBurn();
        }
        else
        {
            //if (Input.GetKeyDown(KeyCode.R)) if (GameManager.Instance.GetLastCheckPoint() != null) KillPlayer(0f);

            //if (Input.GetKeyDown(KeyCode.Space)) isSpacePressed = true;
            //playerMoveDirection = (playerCam.right * -Input.GetAxis("Horizontal")) + (playerCam.forward * -Input.GetAxis("Vertical"));
            //Debug.Log(playerCam.right + " | " + playerCam.forward);
            Vector2 moveDirection = playerInputActions.Player.Move.ReadValue<Vector2>();
            playerMoveDirection = (playerCam.right * -moveDirection.x) + (playerCam.forward * -moveDirection.y);

            PlayerAttachmentsMovement();
            ChangePlayerMaterialDisolve();
        }
    }

    #region PlayerInputs
    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSpacePressed = true;
    }

    public void PlayerRestart(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.Instance.GetLastCheckPoint() != null)
            KillPlayer(0f);
    }

    public void PlayerLevelRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
            Debug.Log("Level Restart Input");
    }
    #endregion

    private void FixedUpdate()
    {
        if (isKilled)
        {
            RespawnPlayer();
            sphereRigidBody.linearDamping = 1.7f;
        }
        else
        {
            IsPlayerBelowMap();

            IsPlayerOnLayer(); //Check ice and lava

            IsGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), 0.2f, groundLayer);
            sphereRigidBody.linearDamping = isGrounded ? (isOnIce ? 0.2f : drag) : 0.1f;
            if (!isGrounded)
                PlayFallingSound();
            else
                PlayerSphereMovement();
        }
    }

    private void PlayerSphereMovement()
    {
        //Debug.DrawLine(transform.position, transform.position + direction * speed * speedForce, Color.green);
        sphereRigidBody.AddForce(speed * speedinterval * (isOnSpeedBoost ? 4f : 1f) * playerMoveDirection);
        if (isSpacePressed)
        {
            sphereRigidBody.AddForce(speedinterval * (isOnJumpBoost ? 8f : 4f) * new Vector3(0f, jumpHeight, 0f));
            isSpacePressed = false;
        }
    }

    private void PlayerAttachmentsMovement() => attachments.position = transform.position;

    private void ChangePlayerMaterialDisolve() //Animate material Disolve
    {
        float th = sphereMaterial.GetFloat("_TreshHold");
        var main = groundParticles.main;
        //sphereLocalMaterial.SetColor("_EmissionColor", isGrounded ? new Color(13f, 0f, 0f, 1f) : new Color(1f, 0f, 0f, 1f));
        sphereMaterial.SetColor("_EmissionColor", glowColor);
        if (isGrounded)
        {
            sphereMaterial.SetFloat("_TreshHold", th < 0.25f ? th + (animationDuration * Time.deltaTime) : 0.25f);
            main.maxParticles = 5;
        }
        else
        {
            sphereMaterial.SetFloat("_TreshHold", th > 0f ? th - (animationDuration * Time.deltaTime) : 0f);
            main.maxParticles = 0;
        }
        burnColorValue = 0f;
    }

    private float burnColorValue = 0f;
    private void ChangePlayerMaterialBurn() //Animate material Burn
    {
        float th = sphereMaterial.GetFloat("_TreshHold");
        var main = groundParticles.main;
        burnColorValue = burnColorValue + Time.deltaTime;
        sphereMaterial.SetColor("_EmissionColor", new Color(13f, burnColorValue, 0f, 1f));
        sphereMaterial.SetFloat("_TreshHold", th < 1f ? th + (animationDuration * Time.deltaTime) : 1f);
        main.maxParticles = 0;
    }

    #region Player Death
    private void IsPlayerBelowMap()
    {
        if (transform.position.y < restartVelocity)
            KillPlayer(0f); //Ustawiæ zmienn¹ bool, a wywy³aæ w Update..
    }

    private void IsPlayerOnLayer()
    {
        if (Physics.CheckSphere(transform.position, 0.5f, deathLayer) && !isKilled)
        {
            audioSourceMain.clip = lavaBurn;
            audioSourceMain.volume = 1f;
            audioSourceMain.pitch = Random.Range(0.9f, 1.2f);
            audioSourceMain.spatialBlend = 0.7f;
            audioSourceMain.Play();
            KillPlayer(2f);
        }

        isOnIce = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), 0.1f, iceLayer);
    }

    private bool isKilled = false;
    private void KillPlayer(float time)
    {
        if (isKilled) return; //Prevent multiple [R] respawns
        isKilled = true;
        playerDied.Invoke();
        StartCoroutine(RespawnPlayerAfterTime(time));
    }

    private bool respawned = false;
    private IEnumerator RespawnPlayerAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        //sphereRigidBody.Sleep();
        
        audioSourceMain.clip = teleportSound;
        audioSourceMain.volume = 0.8f;
        audioSourceMain.pitch = Random.Range(0.9f, 1.2f);
        audioSourceMain.spatialBlend = 0.7f;
        audioSourceMain.Play();
        respawned = true;
    }

    private void RespawnPlayer()
    {
        if (!respawned) return;

        sphereRigidBody.linearVelocity = Vector3.zero;
        sphereRigidBody.angularVelocity = Vector3.zero;
        transform.position = GameManager.Instance.GetLastCheckPointPosition();
        respawned = false;
        isKilled = false;
    }
    #endregion

    private Vector3 lastPlayerPos;
    private void PlayFallingSound()
    {
        if (audioSourceFalling == null) return;
        Vector3 p1 = transform.position;
        Vector3 p2 = lastPlayerPos;
        p1.x = p1.z = p2.x = p2.z = 0f;
        float distance = Vector3.Distance(p1, p2) * 2;
        audioSourceFalling.volume = (distance > 1f ? 1f : distance);
        lastPlayerPos = transform.position;
    }
}
