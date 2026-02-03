using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelTitleLabel;
    [SerializeField] private UISettings soundSettings;
    [SerializeField] private Button leaveButton;
    void Start()
    {
        levelTitleLabel.text = SceneManager.GetActiveScene().name.Replace("Level", "Level ");
    }
}
