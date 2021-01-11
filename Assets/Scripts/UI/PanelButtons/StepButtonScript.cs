using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PanelButtons
{
    public class StepButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var commands = GameObject.Find("SolutionPanel").GetComponent<CodePanel>().GetCommands();
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().StepOnce(commands);
        }
    }
}
