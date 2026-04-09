using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    //attach this script to an empty GO
    //this script holds a list of puzzle events in the scene
    //listens to each oncompleted
    //fires onallsolved when every puzzle is done
    
    //drag each puzzle event game object in the scene into this list
    public PuzzleEvent[] puzzles;
    //fired when every puzzle in the list is solved
    public UnityEvent OnAllSolved;
    //track when puzzles are solved
    private bool[] solvedStates;
    private bool allSolved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(puzzles == null || puzzles.Length == 0)
        {
            Debug.Log("puzzle manager has no puzzles assigned");
            return;
        }

        solvedStates = new bool[puzzles.Length];

        //subscribe to each puzzles oncompleted and onreset events
        //we use a local copy of i for the lambda to capture the right index
        for(int i = 0; i < puzzles.Length; i++)
        {
            int index = 0;
            puzzles[i].OnCompleted.AddListener(() => OnPuzzleSolved(index));
            puzzles[i].OnReset.AddListener(() => OnPuzzleReset(index));
        }
        
    }

    //call when a puzzle event fires oncompleted
    void OnPuzzleSolved(int index)
    {
        Debug.Log("solved states: " + solvedStates[index]);
        solvedStates[index] = true;
        Debug.Log($"Puzzle {index} solved. Checking all puzzles....");
        
        CheckAllSolved();
    }

    //calls when a puzzle event fires onreset
    void OnPuzzleReset(int index)
    {
        solvedStates[index] = false;
        allSolved = false;
        Debug.Log($"Puzzle {index} reset");
    }

    //loops through all solved states and fires OnAllSolved
    //if everyone is true aka completed
    void CheckAllSolved()
    {
        foreach(bool state in solvedStates)
        {
            if (!state) return; //if one of the puzzle states is false aka not completed exit function
        }

        if (!allSolved)
        {
            Debug.Log("check if solved");
            allSolved = true;
            Debug.Log("all puzzles solved");
            OnAllSolved?.Invoke();
            Debug.Log("invoke all solved");
        }
    }

}
