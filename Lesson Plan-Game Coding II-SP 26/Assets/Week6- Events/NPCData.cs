using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("Speaker")]
    public string displayName;
    
    [Header("Dialogue")]
    [TextArea(3, 10)] 
    public string [] lines;
    
    [Header(("If there are choices, we show buttons after line ends"))]
    public DialogueChoice[] choices;
    
    [Header("If no choices, auto continue to this node")]
    public NPCData nextNode;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public NPCData nextNode;
}
