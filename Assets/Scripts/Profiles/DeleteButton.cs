using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var deletePanel = transform.root.Find("DeletePanel");
        //deletePanel.gameObject.SetActive(true);
        deletePanel.GetComponent<FadingPanel>().Show();
        deletePanel.GetComponentInChildren<AcceptProfileDeletionButton>().index = index;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("MouseOn", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("MouseOn", false);
    }
}