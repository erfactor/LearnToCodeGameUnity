using UnityEngine;
using UI;

public class RaycastManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetRaycastBlockingOnInstructionDragged()
    {
        GameObject.Find("Trash").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("InstructionBank").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("ButtonPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("SolutionPanelButtons").GetComponent<CanvasGroup>().blocksRaycasts = false;        

        //GameObject.Find("SolutionPanel").GetComponent<CodePanel>().SetRaycastBlockingForAllInstructions(false);
    }

    public static void SetRaycastBlockingAfterInstructionReleased()
    {
        GameObject.Find("Trash").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("InstructionBank").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("ButtonPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("SolutionPanelButtons").GetComponent<CanvasGroup>().blocksRaycasts = true;

        //GameObject.Find("SolutionPanel").GetComponent<CodePanel>().SetRaycastBlockingForAllInstructions(true);
    }

    public static void SetRaycastBlockingOnCodeExecutionStart()
    {
        GameObject.Find("ButtonPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("SolutionPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("SolutionPanelButtons").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("InstructionBank").GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public static void SetRaycastBlockingOnCodeExecutionStop()
    {
        GameObject.Find("ButtonPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("SolutionPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("SolutionPanelButtons").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("InstructionBank").GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public static void SetFocus()
    {
        GameObject.Find("UIPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public static void ReleaseFocus()
    {
        GameObject.Find("UIPanel").GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
