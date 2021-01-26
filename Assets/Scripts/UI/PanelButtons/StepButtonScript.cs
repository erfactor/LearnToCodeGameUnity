using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PanelButtons
{
    public class StepButtonScript : MonoBehaviour, IPointerClickHandler
    {
        private StopButtonScript _stopButton;

        private void Start()
        {
            _stopButton = transform.parent.Find("StopButton").GetComponent<StopButtonScript>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _stopButton.SetButtonActive();
            
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().StepOnce();
        }
    }
}