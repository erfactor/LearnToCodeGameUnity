using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PanelButtons
{
    public class StepButtonScript : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().StepOnce();
        }
    }
}
