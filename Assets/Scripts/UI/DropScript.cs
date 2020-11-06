using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            //eventData.pointerDrag.transform.position = transform.position;

            if (eventData.pointerDrag.transform.parent != transform)
            {
                eventData.pointerDrag.transform.SetParent(transform);
            }
        }
    }
}
