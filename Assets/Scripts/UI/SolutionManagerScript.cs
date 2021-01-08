using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolutionManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int currentSolutionIndex = 1;

    public void ChangeSolution(int newSolutionIndex)
    {
        if (newSolutionIndex == currentSolutionIndex) return;
        HideSolution(currentSolutionIndex);
        ShowSolution(newSolutionIndex);
        currentSolutionIndex = newSolutionIndex;
    }

    private void HideSolution(int solutionIndex)
    {
        var solutionGameObject = GameObject.Find("LevelCanvas").transform.Find("Panel").transform.Find("SolutionPanel");
        solutionGameObject.name = $"SolutionPanel{solutionIndex}";
        solutionGameObject.SetParent(transform);        
    }

    private void ShowSolution(int solutionIndex)
    {
        var solutionGameObject = transform.Find($"SolutionPanel{solutionIndex}");
        solutionGameObject.name = "SolutionPanel";
        solutionGameObject.SetParent(GameObject.Find("LevelCanvas").transform.Find("Panel"));
    }
}
