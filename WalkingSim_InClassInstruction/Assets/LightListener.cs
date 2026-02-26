using UnityEngine;

public class LightListener : MonoBehaviour
{
    //listener script is like our twitter followers or blog subscribers
    public Light sceneLight;

    //when this object becomes active it is saying when your event fires call my change light function
    public void OnEnable()
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
}
