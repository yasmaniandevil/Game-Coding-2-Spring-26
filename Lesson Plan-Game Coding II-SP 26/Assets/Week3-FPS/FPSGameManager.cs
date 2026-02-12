using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class FPSGameManager : MonoBehaviour
{
    public static FPSGameManager Instance;
    
    //players current score
    public int score = 0;
    //game time settings
    public int gameLength = 40; //total time in seconds
    public float timer = 0; //tracks elapsed time
    public TextMeshProUGUI displayText;
    private bool inGame = true;
    
    //file path variable for saving high scores
    //const is used to define variable that never changes we use it to represent fixed file paths
    const string DIR_DATA = "/Data/";
    const string FILE_HIGH_SCORE = "HighScore.txt"; //file name
    private string PATH_HIGH_SCORE; //full path to the high score file
    
    //list to store high scores
    public List<int> highScoreList = new List<int>();
    public TextMeshProUGUI healthText;
    FPSHealth playerHealth;
    

    public int Score
    {
        get
        {
            return score; //read current score
        }
        set
        {
            score = value; //update current score
            Debug.Log("score changed");
        }
    }

    private void Awake()
    {
        //checks if there already is a game manager
        if (Instance == null)
        {
            //if not then it assigns this one/the current one
            Instance = this;
            //keeps game manager alive when switching scenes
        }
        else
        {
            //destroys duplicated game managers if they exist
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0; //reset game timer
        
        //construct the full file path for savig high scores
        PATH_HIGH_SCORE = Application.dataPath + DIR_DATA + FILE_HIGH_SCORE;;
        Debug.Log("high score file path: "  + PATH_HIGH_SCORE);
        
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSHealth>();
        playerHealth.OnHealthChanged += UpdateHealthText;
        UpdateHealthText(playerHealth.maxHealth, playerHealth.maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
        }

        //if game is still running update timer
        if (inGame)
        {
            timer += Time.deltaTime;
            displayText.text = "Timer: " + (gameLength - (int)timer);
        }
        
        //end game when timer reaches the game length
        if(timer >= gameLength && inGame)
        {
            inGame = false;
            Debug.Log("Game Over");
            //scenemanager.loadScene ("game over screen
            //or turn on UI turn off game time.time = 0);
            UpdateHighScore(); //save high scores when game ends
        }

    }

    private void UpdateHighScore()
    {
        //take the high score out of the file and put them in a list
        
        //load high scores from the file if it exists
        if (File.Exists(PATH_HIGH_SCORE))
        {
            Debug.Log("high score file found, reading data!!");
            
            //get the high scores from the file as a string
            string fileContent = File.ReadAllText(PATH_HIGH_SCORE);
            //n means move to the next line
            Debug.Log("File Contents Before Processing: \n" + fileContent);
            
            //split the string up into any array
            string[] fileSplit = fileContent.Split('\n');
            
            //clear the existing high score list before adding new scores
            //if we dont it will keep adding scores indefinitely 
            highScoreList.Clear();
            
            //convert each string into an integer and add it to the high score list
            foreach (string scoreString in fileSplit)
            {
                if (int.TryParse(scoreString, out int scoreValue))
                {
                    highScoreList.Add(scoreValue);
                    Debug.Log("high score added: " + scoreValue);
                }
            }
        }
        
        //.count returns numbers of elements in the list
        //length returns total size of the array including unused elemnts
        
        //if high score list is empty, add a default score (prevents errors)
        if (highScoreList.Count == 0)
        {
            highScoreList.Add(0);
        }
        
        Debug.Log("current high schore list before adding new score: " + string.Join(",", highScoreList));
        
        //insert the new score into the correct position in the high score list
        if (!highScoreList.Contains(Score))//prevents duplicate high scores that arent in the list b4 inserting them
        {
            //goes thru each score in the list to find where new score should be placed
            for (int i = 0; i < highScoreList.Count; i++)
            {
                //ex: current list is 100, 90, 80, and the new score is 85
                //85 > 80 so it should go before
                if (highScoreList[i] < Score)//find where the new score fits
                {
                    //moves everything after i one position down and puts score at index i
                    highScoreList.Insert(i, Score);
                    Debug.Log($"Inserted new score {Score} at position {i}");
                    break; //stops checking once the score is inserted without this it would continue even tho the new score was added
                }
            }
        }
        
        //if we have more than 5 high scores
        if (highScoreList.Count > 5)
        {
            //cut it to 5 scores
            Debug.Log("high score list exceeding 5 entries");
            highScoreList.RemoveRange(5, highScoreList.Count - 5);
            
        }
        
        Debug.Log("final high score list b4 saving: " + string.Join(",", highScoreList));
        
        //make a string of all our high scores
        string highScoreString = "High Score \n";

        for (int i = 0; i < highScoreList.Count; i++)
        {
            highScoreString += highScoreList[i] + "\n";
        }
        
        //display high scores
        displayText.text = highScoreString;
        
        //write high score to the file
        File.WriteAllText(PATH_HIGH_SCORE, highScoreString);
        Debug.Log("high scores written");
    }

    public void UpdateHealthText(float oldhealth, float newhealth)
    {
        healthText.text = "Health:" + newhealth;
    }
}
