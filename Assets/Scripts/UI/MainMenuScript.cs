using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private int levelsCount = 1;
    [SerializeField] private Transform levelSelectPanel;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject cheatsGO;
    [SerializeField] private GameObject testLevelButton;
    [SerializeField] private AudioSource audioSourceStart;
    [SerializeField] private AudioSource audioSourceLoop;
    static bool isCheatsOn = false;
    private int page = 0;
    private int lastActiveLevel = 1;
    private Animator animator;
    private float[] xCorginates = new float[] { -3.5f, -1.75f, 0f, 1.75f, 3.5f };

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        lastActiveLevel = PlayerPrefs.GetInt("LastSavedLevel") != 0 ? PlayerPrefs.GetInt("LastSavedLevel") : 1;
        animator = GetComponent<Animator>();
        if (isCheatsOn)
            lastActiveLevel = 100;
        StartCoroutine(CreateLevelButtons());
        testLevelButton.SetActive(false);
    }

    private void Update()
    {
        if (audioSourceLoop.isPlaying) return;
        if (!audioSourceStart.isPlaying)
        {
            audioSourceStart.enabled = false;
            audioSourceLoop.Play();
        }
    }

    public void CheatsOn()
    {
        cheatsGO.GetComponent<Image>().color = Color.red;
        isCheatsOn = true;
        lastActiveLevel = 100;
        testLevelButton.SetActive(true);
        StartCoroutine(CreateLevelButtons());
    }

    public void LoadTestLevel() => GameManager.Instance.LoadLevel("Level0");
    public void QuitGame() => Application.Quit();

    public void MoveToLevelPick(bool value) => animator.SetBool("IsInLvlPick", value);
    public void MoveToSettings(bool value) => animator.SetBool("IsInSettings", value);
    public void MoveToPlayerCustomization(bool value) => animator.SetBool("IsInPlayerCustom", value);

    public void ChangePage(bool addPage = true)
    {
        int _page = page;
        _page = addPage ? _page + 1 : _page - 1;
        if (_page < 0 || (float)levelsCount / 10f <= _page) return;
        page = _page;
        StartCoroutine(CreateLevelButtons(page));
    }

    private IEnumerator CreateLevelButtons(int page = 0)
    {
        foreach (Transform child in levelSelectPanel) Destroy(child.gameObject);
        int firstPage = (10 * page) + 1;
        int rowCell = 0;
        for (int i = firstPage; i <= levelsCount; i++)
        {
            if (i < firstPage + 10)
            {
                GameObject button = Instantiate(levelButtonPrefab);
                ButtonLevelSelect buttonLevelSelect = button.GetComponent<ButtonLevelSelect>();
                buttonLevelSelect.LevelNumber = i;
                buttonLevelSelect.RecordTime = PlayerPrefs.GetFloat("Level" + i + "_TimeRecord");
                button.transform.SetParent(levelSelectPanel);
                bool posFirstRow = rowCell < 5;
                button.transform.localPosition = new Vector3(xCorginates[posFirstRow ? rowCell : rowCell - 5], (posFirstRow ? 1f : -1f), 0f);
                if (i > lastActiveLevel) button.GetComponent<Button>().interactable = false;
                rowCell++;
            }
            else yield break;
            yield return null;
        }
    }
}
