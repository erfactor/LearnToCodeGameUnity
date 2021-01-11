using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PanelButtons
{
    public class PanelButtonAnimation : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Animator Animator;
    
        public void OnPointerClick(PointerEventData eventData)
        {
            Animator.SetTrigger("Click");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Animator.SetBool("Hovered", true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animator.SetBool("Hovered", false);
        }
    }
}
