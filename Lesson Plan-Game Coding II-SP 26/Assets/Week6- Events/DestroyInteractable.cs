using UnityEngine;

public class DestroyInteractable :  Interactable
{
    public override void Interact(CCPlayer player)
    {
        Destroy(gameObject);
        Debug.Log($"Destroyed {this.gameObject.name}");
    }
}
