using Profiles;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeleteButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    ProfileManager profileManager;
    Animator animator;

    public void OnPointerClick(PointerEventData eventData)
    {
        profileManager.DeleteProfile(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("najechane");
        animator.SetBool("MouseOn", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("wyjechane");
        animator.SetBool("MouseOn", false);
    }

    void Start()
    {
        profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        animator = GetComponent<Animator>();
    }
}
