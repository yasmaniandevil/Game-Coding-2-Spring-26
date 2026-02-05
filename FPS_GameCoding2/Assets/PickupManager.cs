using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    //list to hold all pickup instances that the player has picked up
    public List<GameObject> pickupList = new List<GameObject>();

    int currentPickupIndex = -1; //start with no weapon

    // Update is called once per frame
    void Update()
    {
        //key to switch weapons
        //weaponslist.count ensures that there is at least one weapon in the list before attmepting to switch weapons
        if(Input.GetKeyDown(KeyCode.Q) && pickupList.Count > 0)
        {
            //currentpickup + 1 moves it by 1 to the next weapon in the list
            //% has a wrapping effect. if currentindex + 1 equals the length of the list it will reset it to 0 back at the top
            //it wraps back around to the first weapon so we dont get an error
            int newPickUp = (currentPickupIndex + 1) % pickupList.Count;
            SwitchPickup(newPickUp);
        }
    }

    public void AddPickup(GameObject pickupPrefab)
    {
        //add instantiated pickup to our list
        pickupList.Add(pickupPrefab);
        //prevent multiple active pickups at once
        pickupPrefab.SetActive(false);//starts with it disabled

        //if its first on the list activate it
        if(pickupList.Count == 1)
        {
            //call switch pickup to switch our weapon
            //switching it to the first weapon on our list
            SwitchPickup(0);
        }
    }

    void SwitchPickup(int index)
    {
        //deactivate the current active weapon if there is one
        if(currentPickupIndex != -1)
        {
            //ensure when switching weapons the previous one is turned off
            pickupList[currentPickupIndex].SetActive(true);
        }

        //set the new pickup as active and update the current index
        currentPickupIndex = index;
        pickupList[currentPickupIndex].SetActive(true );
    }
}
