using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PanelButtons
{
    public class ReturnButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public int TargetScene;

        public void OnPointerClick(PointerEventData eventData)
        {
            ReturnToMainMenu();
        }

        public void ReturnToMainMenu()
        {
            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(TargetScene);
        }
    }
}