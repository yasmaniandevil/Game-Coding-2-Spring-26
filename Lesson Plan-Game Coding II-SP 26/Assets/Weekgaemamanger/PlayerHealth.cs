using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthBar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerHealthText = GameObject.Find("Canvas/HealthText").GetComponent<TextMeshProUGUI>();
        
        
        //on start we have max health that we grab from game manager
        healthBar.maxValue = GameManager.instance.maxHealth;
        healthBar.value = GameManager.instance.currentHealth;
        
        
        //sub to the event when game manager fires OnHEalthChanged
        //this does not call update display right now it just registers it
        //think of it like giving game manager a phone number and saying
        //call this when something happens
        
        //we are NOT calling the function
        //we are pointing at it
        GameManager.instance.OnHealthChange += UpdateDisplay;
    }

    // Update is called once per frame
    void OnDestroy()
    {
        //always unsubscribe when this object is destroyed
        //if you dont game manager will try to call the method on a dead object
        if (GameManager.instance != null) 
            GameManager.instance.OnHealthChange -= UpdateDisplay;
    }

    private void UpdateDisplay(int newHealth)
    {
        healthBar.value = newHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health"))
        {
            GameManager.instance.TakeDamage(10);
        }
    }
    
    //ontrigger enter fires. 
    //take damage runs in game manager
    //current health gets subtracted
    /*onhealthchanhed?.invoke(currenthealth) actually now
        CALLS our function updatedisplay and passes in the int*/
    //update display(currenthealth) runs in playerhealth
    //health bar.value = new health
}
