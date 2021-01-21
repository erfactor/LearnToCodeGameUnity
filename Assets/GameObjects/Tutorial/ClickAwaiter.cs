using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorial
{
    public class ClickAwaiter : MonoBehaviour, IPointerClickHandler
    {
        public bool hasBeenClicked = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click awaiter detected click.");
            hasBeenClicked = true;
        }
    }
}




