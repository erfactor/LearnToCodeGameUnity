using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolutionWindow : MonoBehaviour, IPointerClickHandler
{
    private void Awake()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("Hide");
    }

    public void Show()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<Animator>().SetTrigger("Show");
    }
}
