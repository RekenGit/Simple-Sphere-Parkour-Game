using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null) _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(GameManager)} instance already exists, destroing duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        if (PlayerPrefs.HasKey("PlayerBaseColorId"))
            customizations.selectedBaseColorId = PlayerPrefs.GetInt("PlayerBaseColorId");
        if (PlayerPrefs.HasKey("PlayerGlowColorId"))
            customizations.selectedGlowColorId = PlayerPrefs.GetInt("PlayerGlowColorId");
        if (PlayerPrefs.HasKey("PlayerMeshId"))
            customizations.selectedMeshId = PlayerPrefs.GetInt("PlayerMeshId");
    }
    #endregion

    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private float yVelocityToRespawn = -30f;
    [SerializeField] private bool isMainMenu = false;
    [SerializeField] private PlayerCustomizationObject customizations;
    private Volume volumeSetting;
    private Bloom bloom;
    private GameObject lastCheckPoint;
    private GameObject mainCamera;
    private DisolveUI disolveUI;
    private float levelTime;
    private CameraMovement cameraMovement;

    public void SetNewCheckPoint(GameObject checkpoint) => lastCheckPoint = checkpoint;
    public AudioClip GetMusicClip() => levelMusic;
    public float GetVelocityToRespawn() => yVelocityToRespawn;
    public GameObject GetLastCheckPoint() => lastCheckPoint;
    public Vector3 GetLastCheckPointPosition() => lastCheckPoint.transform.position + new Vector3(0f, 3f, 0f);
    public GameObject GetMainCamera() => mainCamera;
    public void SetVolumeSetting(float vol) => bloom.intensity.value = vol;
    public PlayerCustomizationObject GetPlayerCustomization() => customizations;

    private void Start()
    {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        volumeSetting = GetComponentInChildren<Volume>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        volumeSetting.profile.TryGet(out bloom);
        if (mainCamera == null)
            StartCoroutine(SetMainCamera());
        else
        {
            disolveUI = mainCamera.GetComponentInChildren<DisolveUI>();
            cameraMovement = mainCamera.GetComponentInParent<CameraMovement>();
        }
    }

    private int ticks = 0;
    private bool isTimerRunning = true;
    private void Update()
    {
        if (isMainMenu || cameraMovement == null || !isTimerRunning) return;

        levelTime += Time.deltaTime;
        if (ticks < 5)
        {
            cameraMovement.UpdateTimerLabel(levelTime);
            ticks = 0;
        }
        ticks++;
    }

    private IEnumerator SetMainCamera()
    {
        yield return new WaitForSeconds(0.3f);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        disolveUI = mainCamera.GetComponentInChildren<DisolveUI>();
        cameraMovement = mainCamera.GetComponentInParent<CameraMovement>();
    }

    public void FinishedLevelSuccessfully()
    {
        isTimerRunning = false;
        string activeSceneKey = SceneManager.GetActiveScene().name + "_TimeRecord";
        Debug.Log(activeSceneKey);
        if (PlayerPrefs.HasKey(activeSceneKey))
        {
            float lastSavedRecordTime = PlayerPrefs.GetFloat(activeSceneKey);
            if (levelTime < lastSavedRecordTime)
                PlayerPrefs.SetFloat(activeSceneKey, levelTime);
        }
        else
            PlayerPrefs.SetFloat(activeSceneKey, levelTime);
    }
    public void LoadLevel(string levelName) => StartCoroutine(LoadLevelAfterTime(levelName));
    private IEnumerator LoadLevelAfterTime(string levelName)
    {
        if (!isMainMenu)
        {
            CameraMovement cameraMovement = mainCamera.GetComponentInParent<CameraMovement>();
            StartCoroutine(cameraMovement.StartMusicCozOfUIClose());
            cameraMovement.StopMusic();
        } 
        disolveUI.DisolveUIToggle();
        yield return new WaitForSeconds(3f);
        if (SceneManager.GetSceneByName(levelName) != null)
            SceneManager.LoadScene(levelName);
        else
            SceneManager.LoadScene("Lobby");
    }
}
