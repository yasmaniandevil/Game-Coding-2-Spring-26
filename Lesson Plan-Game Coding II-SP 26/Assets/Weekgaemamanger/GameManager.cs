using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //this is a "hub pattern" game manager is the only singleton; it stores static references to sub-managers
    //everything flows through one door
    public static GameManager instance;
    public DialogueManager dialogueManager;
    public PuzzleManager puzzleManager; //if you make them static then when you call
                                        //them you would call it directly. always use .instance to get the current instance of game manager
    
    //another pattern you can use is "independent singleton pattern"
    //each manager has its own instance, this is more modular, you don't need the game manager when you only want to use puzzle manager in a scene
    //but this creates a web of singletons, harder to manage
    
    //pause and play vars
    private bool isPaused = false;
    public GameObject pauseMenu;

    //states
    //almost always used in game manager
    //the alternative is a growing pile of bools
    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    };
    public GameState currentState;
    
    //player health
    //we put it here so data and logic can go to game manager and if we change scenes our data stays consistent
    //we separate persistent data that lives in game manager from scene UI which lives in the scene
    //we also separate it so we can call take damage from ANYWHERE because we will always have a game manager
    public int maxHealth = 100;
    public int currentHealth;
    
    public event Action<int> OnHealthChange;
    public event Action OnPlayerDeath;
    
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
        
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Start()
    {
        
        SetState(GameState.Playing);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                break;
            case GameState.GameOver:
                Time.timeScale = 0;
                //show game over screen
                break;
        }
    }
    

    public void OnPlayAndPause(InputAction.CallbackContext context)
    {
        //old way with a bool
        /*if (context.performed)
        {
            isPaused = !isPaused;
        }
        
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);*/

        if (!context.performed) return;
        
        if(currentState == GameState.Playing)
            SetState(GameState.Paused);
        else if(currentState == GameState.Paused)
            SetState(GameState.Playing);
    }

    public void TakeDamage(int damageAmt)
    {
        currentHealth -= damageAmt;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        //fire the event player health ui will recieve this
        OnHealthChange?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
            SetState(GameState.GameOver);
        }
    }

    public void LoadScene(string sceneName)
    {
        //this is where we would add a fade, loading screen or set state to
        //gamestate.loading later
        SetState(GameState.Playing);
        SceneManager.LoadScene(sceneName);
    }
    //Gamemanager.Instance.Reload can call in any script bc this script is static
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LoadScene(SceneManager.GetActiveScene().name);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Debug.Log("reload");
        
    }
    public void QuitGame()=> Application.Quit();
}
