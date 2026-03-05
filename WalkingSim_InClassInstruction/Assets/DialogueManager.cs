using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI lineText;
    public Transform choicesContainer;//parent object where choice buttons will spawn
    public Button choiceButtonPrefab; //[refab for a single choice button

    private NPCData currentNode; //current node we are reading from the ScriptableObject SO
    private int lineIndex; //which line index we currently on, keeping track of the dialogue
    private bool isActive; //are we currently in dialogue?

    //lock the player movement and camera
    private CCPlayer player;

    private void Awake()
    {
        //start with dialogue hidden
        if(dialoguePanel != null) dialoguePanel.SetActive(false);
        ClearChoices();
        //find our player!
        player = FindFirstObjectByType<CCPlayer>();
    }
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
        if (!isActive) return; //if no dialogue is active ignore

        if(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (ChoicesAreShowing()) return; //block only when buttons exist
            Advance();
        }

        //keyboard.current.qKey
    }

    void StartDialogue(NPCData npcData)
    {
        if(npcData == null)
        {
            Debug.Log("NPC Data is Null");
            return;
        }

        //this is where we would lock player camera and movement

        //set state
        currentNode = npcData;
        lineIndex = 0;
        isActive = true;

        if(dialoguePanel != null) dialoguePanel.SetActive(true);
        ShowLine();
    }

    bool HasChoices(NPCData node) //check the data
    {
        //does this dialogue node contain choice data
        return node != null && node.choices != null && node.choices.Length > 0;
    }

    void Advance()
    {
        //if node is finished end dialogue
        if(currentNode == null)
        {
            EndDialogue();
            return;
        }

        //move to next line
        lineIndex++;

        //if we still have lines to read in this node, show the next one
        if(currentNode.lines != null && lineIndex < currentNode.lines.Length)
        {
            //if we have something
            if(lineText != null)
            {
                //takes the text of our TMP obj and changes it to whatever the current line is (dependent on lineindex)
                lineText.text = currentNode.lines[lineIndex];
                return;
            }
        }
        //otherwise we have reached the end
        FinishNode();
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        ClearChoices();
        if(choicesContainer == null || choiceButtonPrefab == null)
        {
            Debug.Log("choices are not wired");
            return;
        }

        foreach(DialogueChoice choice in choices)
        {
            //spawn the button as a child of the container
            Button bttn = Instantiate(choiceButtonPrefab, choicesContainer);

            //set visible button text
            TextMeshProUGUI tmp = bttn.GetComponentInChildren<TextMeshProUGUI>();
            if(tmp != null) tmp.text = choice.choiceText;

            //cache next node in a local var
            NPCData next = choice.nextNode;

            //lambda
            //this is like onclick on our buttons
            //we are saying add a listener when the button is clicked run this function
            bttn.onClick.AddListener(() =>
            {
                Choose(next);
            });
        }
    }

    void FinishNode()
    {
        //1. if choice exists show choices
        //2. else if next node exists continue automatically
        //else end dialogue

        //if choices exist, show them
        if (HasChoices(currentNode))
        {
            ShowChoices(currentNode.choices);
            return;

        }

        //auto continue our text
        if(currentNode.nextNode != null)
        {
            currentNode = currentNode.nextNode;
            lineIndex = 0;
            ShowLine();
            return;
        }

        //end
        EndDialogue();
    }
    void ShowLine()
    {
        //when showing a line we shouldnt be showing choices
        ClearChoices();
        //if no node then end dialogue
        if(currentNode == null)
        {
            EndDialogue();
            return;
        }

        //update the speaker name
        if(displayName != null) displayName.text = currentNode.displayName;
        //if node has no lines treat it as a finished node
        if(currentNode.lines == null || currentNode.lines.Length == 0)
        {
            FinishNode();
            return;
        }

        //clamp index so we never go out of bounds
        lineIndex = Mathf.Clamp(lineIndex, 0, currentNode.lines.Length -1);
        //show line text
        if(lineText != null) lineText.text = currentNode.lines[lineIndex];
    }
    void Choose(NPCData nextNode)
    {
        //remove buttons asap so UI feels responsive
        ClearChoices();

        //if no next node this choice ends the convo
        if(nextNode == null)
        {
            EndDialogue();
            return;
        }

        //otherwise go to the chosen node
        currentNode = nextNode;
        lineIndex = 0;
        ShowLine();
    }

    //are the choices displaying in the UI do we see them on the screen?
    bool ChoicesAreShowing()
    {
        return choicesContainer != null && choicesContainer.childCount > 0;
        
        /*bool showing = choicesContainer != null && choicesContainer.childCount > 0;
        Debug.Log(showing);
        return;*/
    }
    void ClearChoices()
    {
        //if we dont have choice container exit the function
        if (choicesContainer == null) return;

        //for every child of the choice container (which is a button) subtract until we clear them all
        for(int i = choicesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesContainer.GetChild(i).gameObject);
        }
    }

    void EndDialogue()
    {
        //lock player camera
        isActive = false; //no longer in dialogue
        currentNode = null; //we dont have a node next (SO)
        lineIndex = 0;

        ClearChoices();

        //turn off dialogue panel
        if(dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}
