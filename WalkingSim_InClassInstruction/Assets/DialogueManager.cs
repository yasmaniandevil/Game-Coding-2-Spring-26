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

    void StartDialogue(NPCData npcData)
    {
        if(npcData == null)
        {
            Debug.Log("NPC Data is Null");
            return;
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (displayName != null) displayName.text = npcData.displayName;
        if(placeHolderOpeningLine != null) placeHolderOpeningLine.text = npcData.placeHolderOpeningLine;
        Debug.Log($"Dialogue start with {npcData.displayName}: {npcData.placeHolderOpeningLine}");
    }
}
