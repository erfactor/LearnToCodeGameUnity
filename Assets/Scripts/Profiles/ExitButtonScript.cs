using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PanelButtons
{
    public class ExitButtonScript : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            ExitGame();
        }

        public void ExitGame()
        {
            #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPlaying = false;
            #else
                     Application.Quit();
            #endif
        }
    }
}