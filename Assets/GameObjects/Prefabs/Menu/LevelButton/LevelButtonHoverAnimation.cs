using Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.parent.GetComponent<LevelButtonScript>().SpeedUpCog();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.parent.GetComponent<LevelButtonScript>().SlowDownCog();
    }
}
