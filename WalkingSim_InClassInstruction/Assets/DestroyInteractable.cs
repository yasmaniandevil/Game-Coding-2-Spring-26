using UnityEngine;

public class DestroyInteractable : Interactable
{
    public override void Interact(CCPlayer ccplayer)
    {
        Destroy(gameObject);
        Debug.Log("Destroyed: " + gameObject.name);
    }
}
