using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LightListener : MonoBehaviour
{
    [FormerlySerializedAs("lightScene")] public Light sceneLight;

    //when this obj becomes active it is saying
    //when your event fires call my changle light function
    private void OnEnable()
    {
        ButtonEvent.onButtonPressed += ChangeLight;
    }

    //when disabled we remove ourselves from the list
    public void OnDisable()
    {
        ButtonEvent.onButtonPressed -= ChangeLight;
    }

    void ChangeLight()
    {
        sceneLight.color = Random.ColorHSV();
    }
    
    //does the button know the light exists? no
    //does the button know that the UI exists? no
    //so how are they talking?
    
    //stored internally is
    //OnbuttPressed: ChangeLight, UpdateText, PlaySound, 
    //a list of function references
    //when invoke is called it goes down the list
}
