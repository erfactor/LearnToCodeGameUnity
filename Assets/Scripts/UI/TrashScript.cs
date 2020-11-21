using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop - usuwanie obiektu");
        if (eventData.pointerDrag != null)
        {
            //eventData.pointerDrag.transform.position = transform.position;

            CodePanel.Remove(eventData.pointerDrag);
            Destroy(eventData.pointerDrag);
        }
    }
}
