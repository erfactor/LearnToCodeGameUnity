using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Canvas myCanvas;

    private Transform startParent;
    private Vector3 startPosition;

    private bool isReplicator = true;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Replicate()
    {
        var newObject = Instantiate(gameObject);
        newObject.transform.SetParent(gameObject.transform.parent);
        newObject.transform.position = transform.position;
        newObject.name = transform.name;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isReplicator)
        {
            isReplicator = false;
            Replicate();
        }

        Debug.Log("OnBeginDrag");
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.1f;

        //startParent = transform.parent;
        //transform.SetParent(transform.root);
        startPosition = transform.position;

        CodePanel.draggedObject = gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");


        //var newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        //rectTransform.anchoredPosition = newPosition;

        transform.position = Input.mousePosition;

        //Debug.Log($"Mousepos: x:{eventData.position.x} y:{eventData.position.y}, rectPos: x:{rectTransform.anchoredPosition.x}, y:{rectTransform.anchoredPosition.y}");
        //Vector2 pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        //transform.position = myCanvas.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;

        //if (transform.parent == startParent || transform.parent == transform.root)
        //{
        //    transform.position = startPosition;
        //    transform.SetParent(startParent);
        //}

        CodePanel.draggedObject = null;
    }
}
