using System;
using NUnit.Framework;
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

    //called when CC player raises the ondialoguerequested flag
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
        //when showing a line we shouldnt be showing choices
        ClearChoices();
        //if no node end dialogue
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }
        //update speaker name
        if(displayName != null) displayName.text = currentNode.displayName;
        //if node has no lines treat it like a finished node
        if (currentNode.lines == null || currentNode.lines.Length == 0)
        {
            FinishNode();
            return;
        }

        //clamp index so we never go out of bounds
        lineIndex = Mathf.Clamp(lineIndex, 0, currentNode.lines.Length - 1);
        //show line text
        if(lineText != null) lineText.text = currentNode.lines[lineIndex];
    }

    //ui check: are choice buttons currently on the screen
    bool ChoicesAreShowing()
    {
        return choicesContainer != null && choicesContainer.childCount > 0;
        
        //to do a debug we need to store the var first
        /*bool showing = choicesContainer != null && choicesContainer.childCount > 0;
        Debug.Log(showing);
        return showing;*/
    }
    //advances to the next line if we are past the last line finish node
    void Advance()
    {
        //if node is finished end dialogue
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }
        
        //move to next line
        lineIndex++;
        
        //if we still have lines to read in this node, show the next one
        if (currentNode.lines != null && lineIndex < currentNode.lines.Length)
        {
            if (lineText != null)
            {
                lineText.text = currentNode.lines[lineIndex];
                return;
            }
            
        }
        //otherwise we have reached the end
        FinishNode();
    }

    //called after finishing all lines in a node
    void FinishNode()
    {
        /*1. if choice exists show choices
         2. else if next node exists continue auto
         3. else end dialogue
         */
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
    
    //data check: does this node contain choice data?
    bool HasChoices(NPCData node) //checks the data
    {
        //does this dialogue node contain choice data?
        return node != null && node.choices != null && node.choices.Length > 0;
        //Debug.Log(HasChoices(node));
    }

    //creates button per choice inside choiceContainer
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
            //spawn the button as a child of the container
            Button bttn = Instantiate(choiceButtonPrefab, choicesContainer);
            
            //set visible button text
            TextMeshProUGUI tmp = bttn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = choice.choiceText;
            
            //cache next node in a local var 
            NPCData next = choice.nextNode;

            //when button is clicked go to that next node or end if null
            //lambda
            //unity buttons have an event
            //this event stores functions to run when the button is clicked
            //when you do add listener you are saying when button is clicked, run this function
            //delays execution doesnt run asap
            bttn.onClick.AddListener(() =>
            {
                Choose(next);
            });
        }
    }

    //called when player clicks a choice button
    void Choose(NPCData nextNode)
    {
        //remove buttons asap so UI feels responsve
        ClearChoices();
        //if no next node this choice ends the convo
        if (nextNode == null)
        {
            EndDialogue();
            return;
        }
        //otherwise go to the chosen node
        currentNode = nextNode;
        lineIndex = 0;
        ShowLine();
        //Debug.Log($"Choose clicked. nextNode = {(nextNode == null ? "NULL" : nextNode.name)}");
    }
    
    //deletes all spawned choice button children
    void ClearChoices()
    {
        if (choicesContainer == null) return;
        for (int i = choicesContainer.childCount - 1; i >= 0 ; i--)
        {
            Destroy(choicesContainer.GetChild(i).gameObject);
        }
    }

    //end dialogue, hide UI, reset state, unlock player controls
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
