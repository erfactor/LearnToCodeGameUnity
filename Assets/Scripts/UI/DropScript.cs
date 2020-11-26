using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//this is the script that should be associated with the SLOT at which the UI element is dropped, not the draggable UI element.
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
