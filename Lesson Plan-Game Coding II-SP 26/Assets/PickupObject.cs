using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    //the prefab that will be instianted when picked up
    public GameObject pickupObject;

    //the transform of the socket to which the item will be parented to the player
    public Transform pickupSocket;
    // Start is called before the first frame update
    
    PickupManager pickupManager;

    private void Start()
    {
        pickupManager = FindObjectOfType<PickupManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //instantiate and parent to the socket
            GameObject newPickUp = Instantiate(pickupObject, pickupSocket.position, Quaternion.identity, pickupSocket);
            
            //reset local pos and rotation to ensure it fits correctly into the socked
            newPickUp.transform.localPosition = Vector3.zero;
            newPickUp.transform.localRotation = Quaternion.identity;
            
            //adds it to our list
            pickupManager.AddPickup(newPickUp);
            
            //destroyed the pickup game object
            Destroy(gameObject);
            
            Debug.Log("Picked up");
        }
    }
}
