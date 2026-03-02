using System;
using TMPro;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    //event system allows one script to announce that something has happened
    //and other scripts to react-without directly referencing eachother
    //hard referecing is tight coupling where everything is connected
    //doorScript.Open();
    //ui.UpdateScore();
    
    //action = built in delegate
    //it stores functions
    //can be invobed by anyone
    //can be overwrriten
    //event is a special delegate
    //only declaring class can invoke
    //other classes can only sub and unsub
    //?.invoke() = safe call
    
    //a delegate is a variable that can store a function
    //int number vs Action myFunction
    //an event is a protected delegate if you do it without it it can break
    //other scripts can sub and unsub but they can not invoke it
    /*static belongs to the class itself not a specific instance
     meaning we dont need to reference a specific Gameobject 
     you can just += instead of FindObjectOfType<ThisScript>*/
    //there is one shared version across the entire program
    
    //think twitter
    //when you post you dont call each follower directly
    //twitter notifies everyone who has subscribed
    //event = notification syste,
    public static event Action onButtonPressed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPressed()
    {
        //invoke means call every function subscribed to this event
        //.? means only do this if it isnt null(if someone is listening)
        onButtonPressed?.Invoke();
    }
    
    //this isnt a bad way to things its just not "scalable"
    //has direct reference to light and text
    //button script manually controlling them
    /*why this issue later? what if u want sound effect,
     camera shake? enemy spawn? dialogue trigger
     you would have to keep modifying this script*/
    //if you later delete the light now the button breaks
    
    //in real games buttons shouldnt know about enemies
    //enemies shouldnt know about dialogue
    //dialogue shouldnt know about audio etc
    //they should respond to events
    
    /*public void OnButtonPress()
    {
        Light sceneLight = GetComponent<Light>();
        TextMeshProUGUI statusText;
        
        sceneLight.color = Color.green;
        statusText.text = "Pressed";
        
        What happens if I want 10 things to react?
    }*/
}
