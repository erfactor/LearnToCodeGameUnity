using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.PanelButtons
{
    public class PlayButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public Sprite PlayButton;
        public Sprite PauseButton;
        public bool _isGameOn;
        private StopButtonScript _stopButton;

        private void Start()
        {
            _stopButton = transform.parent.Find("StopButton").GetComponent<StopButtonScript>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isGameOn)
            {
                PauseGame();
                GameObject.Find("LevelLoader").GetComponent<LevelLoader>().PauseExecution();
            }
            else
            {
                RaycastManagerScript.SetRaycastBlockingOnCodeExecutionStart();
                GetComponent<Image>().overrideSprite = PauseButton;
                _stopButton.SetButtonActive();
                var commands = GameObject.Find("SolutionPanel").GetComponent<CodePanel>().GetCommands();
                GameObject.Find("LevelLoader").GetComponent<LevelLoader>().StartExecution(commands);
                _isGameOn = true;
            }
        }

        public void PauseGame()
        {
            GetComponent<Image>().overrideSprite = PlayButton;
            _isGameOn = false;
        }
    }
}