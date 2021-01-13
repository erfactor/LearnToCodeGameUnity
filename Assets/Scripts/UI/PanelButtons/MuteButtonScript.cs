using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.PanelButtons
{
    public class MuteButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public Sprite musicOn;
        public Sprite musicOff;
        private SFXManagerScript _sfxManager;

        private void Start()
        {
            _sfxManager = GameObject.Find("SFXManager").GetComponent<SFXManagerScript>();
            GetComponent<Image>().overrideSprite = _sfxManager.SoundMuted ? musicOff : musicOn;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_sfxManager.SoundMuted)
            {
                GetComponent<Image>().overrideSprite = musicOn;
                _sfxManager.UnmuteSound();
            }
            else
            {
                GetComponent<Image>().overrideSprite = musicOff;
                _sfxManager.MuteSound();
            }
        }
    }
}
