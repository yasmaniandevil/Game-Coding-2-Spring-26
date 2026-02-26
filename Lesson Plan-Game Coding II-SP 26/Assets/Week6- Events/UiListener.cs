using System;
using TMPro;
using UnityEngine;

public class UiListener : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    private void OnEnable()
    {
        ButtonEvent.onButtonPressed += UpdateText;
    }

    private void OnDisable()
    {
        ButtonEvent.onButtonPressed -= UpdateText;
    }

    void UpdateText()
    {
        statusText.text = "Button pressed";
    }
    
    //does the button know the text exists? no
    //does the button know that the UI exists? no
    //so how are they talking?
}
