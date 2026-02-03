using TMPro;
using UnityEngine;

public class ButtonLevelSelect : MonoBehaviour
{
    private int levelNumber;
    public int LevelNumber
    {
        get { return levelNumber; }
        set 
        { 
            levelNumber = value;
            string levelName = levelNumber + " Level ";
            name = levelName;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        }
    }

    private float recordTime;
    public float RecordTime
    {
        get { return recordTime; }
        set
        {
            recordTime = value;
            if (value == 0f)
            {
                transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "No Record";
                return;
            }
            int hours = Mathf.FloorToInt(value / 3600F);
            int minutes = Mathf.FloorToInt(value / 60F);
            int seconds = Mathf.FloorToInt(value - minutes * 60);
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format("Record\n{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    public void LoadSceneLevel() => GameManager.Instance.LoadLevel("Level" + levelNumber);
}
