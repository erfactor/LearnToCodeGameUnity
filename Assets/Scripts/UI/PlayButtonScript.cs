using Services;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButtonScript : MonoBehaviour, IPointerClickHandler
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Play button clicked");
        var codePanel = GameObject.Find("SolutionPanel");
        var codePanelScript = codePanel.GetComponent<CodePanel>();
        var commands = codePanelScript.GetCommands();

        GameObject.Find("LevelLoader").GetComponent<LevelLoader>().StartExecution(commands);
    }


}
