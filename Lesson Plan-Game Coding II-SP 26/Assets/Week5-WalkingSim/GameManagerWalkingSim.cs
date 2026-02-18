using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerWalkingSim : MonoBehaviour
{
    //game manager is a script that controls the overall logic of the game
    //is the game playing, is it paused? game over? did the player win?
    
    //static means a variable that belongs to the class itself, rather than to any specific object (instance) of that class
    public static GameManagerWalkingSim instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("reload");
    }
}
