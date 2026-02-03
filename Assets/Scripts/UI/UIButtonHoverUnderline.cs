using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverUnderline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text buttonText;
    private string originalText;
    void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        originalText = buttonText.text;
    }
    public void OnPointerEnter(PointerEventData eventData) => buttonText.text = "<u>" + originalText + "</u>";
    public void OnPointerExit(PointerEventData eventData) => buttonText.text = originalText;
}
