using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.PanelButtons
{
    public class StopButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public Sprite StopButtonDisabled;
        public Sprite StopButtonActive;
        private PlayButtonScript _playButton;

        private void Start()
        {
            _playButton = transform.parent.Find("PlayButton").GetComponent<PlayButtonScript>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RaycastManagerScript.SetRaycastBlockingOnCodeExecutionStop();
            _playButton.PauseGame();
            GetComponent<Image>().overrideSprite = StopButtonDisabled;
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().StopAndReload();
        }
    
        public void SetButtonActive()
        {
            GetComponent<Image>().overrideSprite = StopButtonActive;
        }
    }
}