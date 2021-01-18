using UnityEngine;

public class SolutionManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeSolution(3);
        ChangeSolution(2);
        ChangeSolution(1);
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
        var solutionGameObject = GameObject.Find("LevelCanvas").transform.Find("UIPanel").transform.Find("SolutionPanel");
        solutionGameObject.name = $"SolutionPanel{solutionIndex}";
        solutionGameObject.SetParent(transform);
        solutionGameObject.gameObject.SetActive(false);
    }

    private void ShowSolution(int solutionIndex)
    {
        var solutionGameObject = transform.Find($"SolutionPanel{solutionIndex}");
        solutionGameObject.name = "SolutionPanel";
        solutionGameObject.SetParent(GameObject.Find("LevelCanvas").transform.Find("UIPanel"));
        solutionGameObject.gameObject.SetActive(true);
    }
}
