using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    //list to hold all pickup instances that the player has pickedup

    public List<GameObject> pickupList = new List<GameObject>();
    
    int currentPickUpIndex = -1; //start with no weapon
    
    

    // Update is called once per frame
    void Update()
    {
        //key to switch weapons
        //weaponList.Count ensures that there is at least one weapon in the list before attemptimg to switch weapons
        if (Input.GetKeyDown(KeyCode.Q) && pickupList.Count > 0)
        {
            //+ 1 increments the currentweaponindex by 1 moving to the next weapon in the list
            //% wrapping effect. if currentweaponindex + 1 equals the length of the list weaponslist.count it resets it to 0
            //if player is using last weapon in the list and presses Q it wraps back around to first weapon
            int nextPickUp = (currentPickUpIndex + 1) % pickupList.Count;
            SwitchPickUp(nextPickUp);
            
        }
    }

    public void AddPickup(GameObject pickupPrefab)
    {
        //add instantiated pickup to the list
        pickupList.Add(pickupPrefab);
        //prevents multiple active pickups at once
        pickupPrefab.SetActive(false); //start with it disabled

        //if its first one activate it
        if (pickupList.Count == 1)
        {
            SwitchPickUp(0);
        }
        
    }

    //switches to the pickup at specified index
    private void SwitchPickUp(int index)
    {
        //deactivate the current active weapon if there is one
        if (currentPickUpIndex != -1)
        {
            //ensures when switching weapons the previous one is turned off
            pickupList[currentPickUpIndex].SetActive(false);
        }
        
        //set the new pickup as active and update the current index
        currentPickUpIndex = index;
        pickupList[currentPickUpIndex].SetActive(true);
    }
    
}
