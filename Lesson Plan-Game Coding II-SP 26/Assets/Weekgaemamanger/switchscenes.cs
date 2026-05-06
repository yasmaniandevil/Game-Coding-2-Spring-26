using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchscenes : MonoBehaviour
{
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<FPSPlayer>().GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.LoadScene("GameManagerScene");
            //player.transform.position = new Vector3(0, 1, 0);
            Debug.Log("moved player: " + player.transform.position);
        }
    }
}
