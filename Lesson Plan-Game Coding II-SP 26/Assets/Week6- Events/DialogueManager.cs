using System;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI placeHolderOpeningLine;
    private void OnEnable()
    {
        CCPlayer.OnDialogueRequested += StartDialogue;
    }

    private void OnDisable()
    {
        CCPlayer.OnDialogueRequested -= StartDialogue;
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }*/
    }

    void StartDialogue(NPCData npcData)
    {
        if (npcData == null)
        {
            Debug.Log("NPCData is null");
            return;
        }
        if(dialoguePanel != null) dialoguePanel.SetActive(true);
        if(displayName != null) displayName.text = npcData.displayName;
        Debug.Log($"[Dialogue] start with {npcData.displayName}: {npcData.placeHolderOpeningLine}");
    }
}
