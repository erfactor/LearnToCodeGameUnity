using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FadeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger("Hover");
        Debug.Log("Hover fade button");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetTrigger("Unhover");
        Debug.Log("unhover fade button");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
