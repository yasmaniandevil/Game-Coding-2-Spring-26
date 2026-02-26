using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Data")]
public class NPCData : ScriptableObject
{
    public string displayName;
    //[TextArea(3, 10)] public string lines;
    public string placeHolderOpeningLine;
}
