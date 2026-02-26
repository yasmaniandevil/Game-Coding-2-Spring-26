using TMPro;
using UnityEngine;

public class UiListener : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public void OnEnable()
    {
        ButtonEvent.onButtonPressed += UpdateText;
    }

    public void OnDisable()
    {
        ButtonEvent.onButtonPressed -= UpdateText;
    }
    void UpdateText()
    {
        statusText.text = "Button Pressed";
    }

    //does the button know that the text exits? NO
    //BUT NOW IT IS LISTENING!!!
}
