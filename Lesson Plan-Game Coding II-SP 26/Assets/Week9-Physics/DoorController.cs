using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject door;
    public GameObject audioSourceGO;
   
    AudioSource audioSource;
    private void Awake()
    {
        
         audioSource = audioSourceGO.GetComponent<AudioSource>();
         
    }

    public void Open()
    {
        door.SetActive(false);
        audioSource.Play();
    }

    public void Close()
    {
        door.SetActive(true);
    }
}
