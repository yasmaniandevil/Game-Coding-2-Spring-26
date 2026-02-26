using UnityEngine;

public class NPCInteractable : Interactable
{
    public NPCData data;

    public override void Interact(CCPlayer player)
    {
        if (data == null)
        {
            Debug.Log($"NPC {this.gameObject.name} has no data");
        }
        
        player.RequestDialogue(data);
    }
}
