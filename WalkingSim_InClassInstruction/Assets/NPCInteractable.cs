using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public NPCData npcData;

    public override void Interact(CCPlayer ccplayer)
    {
        if(npcData == null)
        {
            Debug.Log("npc has no data: " + gameObject.name);
        }

        //if we are interacting with the npc and it has data then request dialogue 
        ccplayer.RequestDialogue(npcData);
    }
}
