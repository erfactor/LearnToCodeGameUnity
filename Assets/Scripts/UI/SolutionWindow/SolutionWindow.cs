using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolutionWindow : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("Hide");
    }

    public void Show()
    {
        GetComponent<Animator>().SetTrigger("Show");
    }
}
