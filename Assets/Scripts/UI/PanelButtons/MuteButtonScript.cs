using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.PanelButtons
{
    public class MuteButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public Sprite musicOn;
        public Sprite musicOff;

        private void Start()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var sfxManager = GameObject.Find("SFXManager").GetComponent<SFXManagerScript>();
            if (sfxManager.SoundMuted)
            {
                GetComponent<Image>().overrideSprite = musicOn;
                sfxManager.UnmuteSound();
            }
            else
            {
                GetComponent<Image>().overrideSprite = musicOff;
                sfxManager.MuteSound();
            }
        }
    }
}
