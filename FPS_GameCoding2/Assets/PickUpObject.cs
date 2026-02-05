using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    //the prefab that will be instantiated when picked up
    public GameObject pickupObject;

    //the transform of the socket to which the item will be parented to the player
    public Transform pickupSocket;

    //we are making a reference to our manager, so we can later on call a function from it
    PickupManager pickupManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pickupManager = Object.FindFirstObjectByType<PickupManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the player is walking into our trigger
        if (other.CompareTag("Player"))
        {
            //instantiate and parent it to the socket
            //inside instantiate(game object, where we want it to spawn, no rotation, what to parent it to)
            GameObject newPickUp = Instantiate(pickupObject, pickupSocket.position, Quaternion.identity, pickupSocket);

            //reset local position and rotation to ensure it first correctly into the socket
            newPickUp.transform.localPosition = Vector3.zero;
            newPickUp.transform.localRotation = Quaternion.identity;

            //add it to our list
            pickupManager.AddPickup(newPickUp);

            //destroy the representation of the weapon
            Destroy(gameObject);

            Debug.Log("Picked up!");
        }
    }
}
