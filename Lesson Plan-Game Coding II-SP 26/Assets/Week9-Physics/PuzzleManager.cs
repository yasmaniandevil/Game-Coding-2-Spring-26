using System;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    //atach to empty gameobject in scene
    //this script holds a list of puzzle events in the scene
    //listens to each one to complete
    //fires onallsolved when every puzzle is done
    //drag each puzzle event game object into the array
    
    //drag all puzzle event gameobjects in the scene into this list
    public PuzzleEvent[] puzzles;
    //fired when every puzzle in the list is solved
    public UnityEvent OnAllSolved;
    //tracks when puzzles are solved
    private bool[] solvedStates;
    private bool allSolved = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (puzzles == null || puzzles.Length == 0)
        {
            Debug.LogWarning("PuzzleManager has no puzzles assigned!");
            return;
        }
 
        solvedStates = new bool[puzzles.Length];
 
        // Subscribe to each puzzle's OnCompleted and OnReset events
        // We use a local copy of i for the lambda to capture the right index
        for (int i = 0; i < puzzles.Length; i++)
        {
            int index = i; // capture for lambda
            puzzles[i].OnCompleted.AddListener(() => OnPuzzleSolved(index));
            puzzles[i].OnReset.AddListener(() => OnPuzzleReset(index));
        }
    }

   
    //called when a puzzle event fires oncompleted
    void OnPuzzleSolved(int index)
    {
        solvedStates[index] = true;
        Debug.Log($"Puzzle {index} solved. Checking all puzzles....");
        CheckAllSolved();
    }

    //calls when a puzzle event fires onreset
    void OnPuzzleReset(int index)
    {
        solvedStates[index] = false;
        allSolved = false;
        Debug.Log($"Puzzle {index} reset.");
    }

    //loops through all solved states and fires OnAllSolved
    //if every single one is true
    void CheckAllSolved()
    {
        foreach (bool state in solvedStates)
        {
            if (!state) return;
        }

        if (!allSolved)
        {
            allSolved = true;
            Debug.Log("all puzzles solved.");
            OnAllSolved?.Invoke();
        }
    }
}
