using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //static means a variable that belongs to the class itself rathere than the specific instance of that class
    public static GameManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //if we dont have dont have a gamemanager in the next scene then dont destroy this one
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //but if we go into the next scene and we do have a game manager than destroy this one!
            Destroy(gameObject);
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        //if we click the button reload the scene we are in
        if(context.performed) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("reload");
    }
}
