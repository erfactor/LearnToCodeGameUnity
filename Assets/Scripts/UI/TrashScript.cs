﻿using UnityEngine;
using UnityEngine.EventSystems;
using UI;

public class TrashScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop - usuwanie obiektu");
        if (eventData.pointerDrag != null)
        {
            //eventData.pointerDrag.transform.position = transform.position;            

            Delete(eventData.pointerDrag);
            //CodePanel.Remove(eventData.pointerDrag);

            //GameObject.Find("RaycastManager").GetComponent<RaycastManagerScript>().SetRaycastBlockingAfterInstructionReleased();
            RaycastManagerScript.SetRaycastBlockingAfterInstructionReleased();
        }
    }

    public void Delete(GameObject go)
    {
        GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayInstructionDeleteSound();
        GameObject.Find("SolutionPanel").GetComponent<CodePanel>().Remove(go);
    }
}
