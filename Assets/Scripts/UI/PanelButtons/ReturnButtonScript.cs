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
            Destroy(GameObject.Find("TileLevel"));
            Destroy(GameObject.Find("LevelLoader"));
            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(TargetScene);
        }
    }
}