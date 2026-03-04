using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /*Dialogue Manager Job:
     -Listen for start dialogue requests from the player (event
     -display lines, one by one, space advances
     when line ends
     if choces exist, spawn buttons
     else if next node exists auto continue
     else end dialogue
     optionally locks/unlocks player controls while dialogue is active*/
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI lineText;
    
    [Header("Choices")]
    public Transform choicesContainer; //parent object where choice buttons will spawn
    public Button choiceButtonPrefab; //prefab for single choice button

    //current node we are reading from SO
    private NPCData currentNode;
    //which line index we currently on
    private int lineIndex;
    //are we currently in dialogue mode?
    private bool isActive;
    
    //ref to the player
    private CCPlayer player;

    private void Awake()
    {
        //start w dialogue hidden
        if(dialoguePanel != null)dialoguePanel.SetActive(false);
        //clear any leftover buttons 
        ClearChoices();
        //grab player ref
        player = FindFirstObjectByType<CCPlayer>();
    }

    private void OnEnable()
    {
        //subscribe to event when this object becomes enabled/active
        //now dialogue manager is listening for dialogue requests
        CCPlayer.OnDialogueRequested += StartDialogue;
    }

    private void OnDisable()
    {
        CCPlayer.OnDialogueRequested -= StartDialogue;
    }

    private void Update()
    {
        if (!isActive) return; //if no dialogue is active, ignore
        
        //space advances only if we are not showing choices
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (ChoicesAreShowing()) return; //block only when buttons exist
            Advance();
        }
        //other option instead of space
        //if(Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
    }

    void StartDialogue(NPCData npcData)
    {
        
        if (npcData == null)
        {
            Debug.Log("NPCData is null");
            return;
        }
        
        //lock movement and mouse
        if(player != null) player.SetControlsLocked(true);
        
        //set state
        currentNode = npcData;
        lineIndex = 0;
        isActive = true;
        
        //show UI
        if(dialoguePanel != null) dialoguePanel.SetActive(true);
        
        //show first line
        ShowLine();
    }

    //display the current line based on current node + lineindex
    void ShowLine()
    {
        ClearChoices();
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }
        
        if(displayName != null) displayName.text = currentNode.displayName;
        //if node has no lines treat it like a finished node
        if (currentNode.lines == null || currentNode.lines.Length == 0)
        {
            FinishNode();
            return;
        }

        lineIndex = Mathf.Clamp(lineIndex, 0, currentNode.lines.Length - 1);
        if(lineText != null) lineText.text = currentNode.lines[lineIndex];
    }

    bool ChoicesAreShowing()
    {
        return choicesContainer != null && choicesContainer.childCount > 0;
    }
    void Advance()
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }
        
        lineIndex++;
        
        //still reading lines in this node
        if (currentNode.lines != null && lineIndex < currentNode.lines.Length)
        {
            if (lineText != null)
            {
                lineText.text = currentNode.lines[lineIndex];
                return;
            }
            
        }
        
        FinishNode();
    }

    void FinishNode()
    {
        //Debug.Log($"FinishNode on {currentNode.name} | nextNode = {(currentNode.nextNode ? currentNode.nextNode.name : "NULL")} | choices = {(currentNode.choices != null ? currentNode.choices.Length : 0)}");
        //if choices exist, show them
        if (HasChoices(currentNode))
        {
            ShowChoices(currentNode.choices);
            return;
        }
        
        //auto continue text
        if (currentNode.nextNode != null)
        {
            currentNode = currentNode.nextNode;
            lineIndex = 0;
            ShowLine();
            return;
        }
        
        //end
        EndDialogue();
    }
    
    bool HasChoices(NPCData node) //checks the data
    {
        //does this dialogue node contain choice data?
        return node != null && node.choices != null && node.choices.Length > 0;
    }

    //are the buttons currently on the screen?
    void ShowChoices(DialogueChoice[] choices)
    {
        ClearChoices();
        if (choicesContainer == null || choiceButtonPrefab == null)
        {
            Debug.Log("Choices are not wired");
            return;
        }

        foreach (DialogueChoice choice in choices)
        {
            Button bttn = Instantiate(choiceButtonPrefab, choicesContainer);
            
            TextMeshProUGUI tmp = bttn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = choice.choiceText;

            NPCData next = choice.nextNode;

            bttn.onClick.AddListener(() =>
            {
                Choose(next);
            });
        }
    }

    void Choose(NPCData nextNode)
    {
        ClearChoices();
        if (nextNode == null)
        {
            EndDialogue();
            return;
        }

        currentNode = nextNode;
        lineIndex = 0;
        ShowLine();
        Debug.Log($"Choose clicked. nextNode = {(nextNode == null ? "NULL" : nextNode.name)}");
    }
    void ClearChoices()
    {
        if (choicesContainer == null) return;
        for (int i = choicesContainer.childCount - 1; i >= 0 ; i--)
        {
            Destroy(choicesContainer.GetChild(i).gameObject);
        }
    }

    void EndDialogue()
    {
        if(player != null) player.SetControlsLocked(false);
        isActive = false;
        currentNode = null;
        lineIndex = 0;
        
        ClearChoices();
        
        if(dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}
