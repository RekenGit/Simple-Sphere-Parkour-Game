using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject playerCamera;
    [SerializeField][Range(0, 1)] private float smoothnes;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 offsetCamUp;
    [SerializeField] private Canvas UIPanel;
    [SerializeField] private Canvas UIGame;
    [SerializeField] private GameObject[] MobileUIs;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource bacgroundUISound;
    [SerializeField] private TMP_Text timerLabel;
    private PlayerControls playerControls;
    private Animator musicAnimator;
    private Vector3 velocity = Vector3.zero;
    private float mouseMove;
    private bool isUIOpen = false;
    private LayerMask camUpLayer;
    [SerializeField] private LayerMask camDownLayer;
    private bool setCamPosUp;
    private bool SetCamPos
    {
        get { return setCamPosUp; }
        set
        {
            if (setCamPosUp != value)
            {
                if (!setCamPosUp || Physics.CheckCapsule(player.position - new Vector3(0f, 3.4f, 0f), player.position - new Vector3(0f, 6.4f, 0f), 3f, camDownLayer))
                    setCamPosUp = value;
                else
                    return;

                StartCoroutine(SetCameraRotationAfterSec(0.1f));
            }
        }
    }

    private void Start()
    {
        foreach (GameObject MobileUI in MobileUIs)
            MobileUI.SetActive(false);
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            //player.GetComponent<PlayerMovement>().playerDied.AddListener(UIToggled);
            playerControls = player.GetComponent<PlayerMovement>().GetPlayerInputActions();
            playerControls.Player.OpenCloseUI.performed += PlayerUI;
            playerControls.Player.Move.performed += PlayerStartMoving;
            playerControls.Player.Move.canceled += PlayerStopMoving;
            playerControls.Player.ScreenSwipePrimary.performed += PlayerTouchScreenPrimary;
            playerControls.Player.ScreenSwipeSecond.performed += PlayerTouchScreenSecond;
        }

        UIPanel.enabled = isUIOpen;
        UIGame.enabled = !isUIOpen;
        camUpLayer = LayerMask.GetMask("CamUp");
        musicAnimator = GetComponent<Animator>();

        SetMusic(GameManager.Instance.GetMusicClip());
        SetCameraOffset();

#if UNITY_ANDROID
        foreach (GameObject MobileUI in MobileUIs)
            MobileUI.SetActive(true);
#endif
    }

    public void GoToLobbyScene() => GameManager.Instance.LoadLevel("Lobby");

    private bool canRotateCamera = true;
    public void PlayerStartMoving(InputAction.CallbackContext context)
    {
        if (context.performed)
            canRotateCamera = false;
    }
    public void PlayerStopMoving(InputAction.CallbackContext context)
    {
        if (context.canceled)
            canRotateCamera = true;
    }

    public void PlayerUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isUIOpen)
                StartCoroutine(StopMusicCozOfUIOpen());
            else
                StartCoroutine(StartMusicCozOfUIClose());
        }
    }

    private void PlayerTouchScreenPrimary(InputAction.CallbackContext context)
    {
#if UNITY_ANDROID
        if (context.performed && canRotateCamera)
        {
            Vector2 swipeDirection = context.ReadValue<Vector2>();
            mouseMove += swipeDirection.x / 2.5f;
        }
#endif
    }

    private void PlayerTouchScreenSecond(InputAction.CallbackContext context)
    {
#if UNITY_ANDROID
        if (context.performed)
        {
            Vector2 swipeDirection = context.ReadValue<Vector2>();
            mouseMove += swipeDirection.x / 2f;
        }
#endif
    }

    public void UpdateTimerLabel(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600F);
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        timerLabel.text = string.Format("Time\n{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    #region MusicControl
    private void SetMusic(AudioClip clip)
    {
        musicPlayer.clip = clip;
        musicPlayer.Play();
        musicAnimator.SetBool("IsMusicSet", true);
    }

    public void StopMusic() => musicAnimator.SetBool("StopMusic", true);

    public IEnumerator StopMusicCozOfUIOpen()
    {
        UIPanel.enabled = isUIOpen = true;
        UIGame.enabled = !isUIOpen;
        bacgroundUISound.Play();
        musicAnimator.SetBool("IsUIOpen", true);
        yield return new WaitForSeconds(0.2f);
        musicPlayer.Pause();
    }

    public IEnumerator StartMusicCozOfUIClose()
    {
        UIPanel.enabled = isUIOpen = false;
        UIGame.enabled = !isUIOpen;
        bacgroundUISound.Pause();
        musicPlayer.Play();
        musicAnimator.SetBool("IsUIOpen", false);
        yield return new WaitForSeconds(0.2f);
    }
    #endregion

    #region CameraOffset
    private IEnumerator SetCameraRotationAfterSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        SetCameraOffset();
    }
    public void SetCameraUpOffset()
    {
        //SetCamPos = Physics.CheckSphere(player.position - new Vector3(0f, 3.4f, 0f), 3f, camUpLayer); //Layer in radious 3f
        SetCamPos = Physics.CheckCapsule(player.position - new Vector3(0f, 3.4f, 0f), player.position - new Vector3(0f, 6.4f, 0f), 3f, camUpLayer);
        SetCameraOffset();
    }

    public void SetCameraOffset()
    {
        playerCamera.transform.localPosition = setCamPosUp ? offsetCamUp : offset;
        playerCamera.transform.LookAt(transform.position);
    }

    #endregion

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (!isUIOpen)
        //        StartCoroutine(StopMusicCozOfUIOpen());
        //    else
        //        StartCoroutine(StartMusicCozOfUIClose());
        //}
        if (!isUIOpen) 
            mouseMove += playerControls.Player.LookMove.ReadValue<Vector2>().x;
        //mouseMove += Input.GetAxis("Mouse X");

#if UNITY_STANDALONE_WIN
        UnityEngine.Cursor.lockState = isUIOpen ? CursorLockMode.None : CursorLockMode.Locked;
#endif
    }

    private int tick = 0;
    void LateUpdate()
    {
        if (isUIOpen) return;
        tick++;

        //Vector3 smothedPosition = Vector3.Lerp(transform.position, player.position, smoothnes);
        transform.localRotation = Quaternion.Euler(0f, mouseMove, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, player.position, ref velocity, smoothnes);

        if (tick > 5) return;
        SetCameraUpOffset();
        tick = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; //Layer camUp
        Gizmos.DrawWireSphere(player.position - new Vector3(0f, 3.4f, 0f), 3f);
        Gizmos.DrawWireSphere(player.position - new Vector3(0f, 6.4f, 0f), 3f);
        //Gizmos.DrawCube(player.position - new Vector3(0f, 3.4f, 0f), new Vector3(3f, 6f, 3f));
    }
}