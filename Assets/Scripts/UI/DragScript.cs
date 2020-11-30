using UnityEngine;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Canvas myCanvas;

    private Transform startParent;
    private Vector3 startPosition;

    public bool IsReplicator { get; set; } = true;

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
        if (IsReplicator)
        {
            IsReplicator = false;
            Replicate();
            transform.SetParent(transform.parent.parent);
        }

        Debug.Log("OnBeginDrag");
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.1f;

        //startParent = transform.parent;
        //transform.SetParent(transform.root);
        startPosition = transform.position;

        CodePanel.draggedObject = gameObject;

        //GameObject.Find("RaycastManager").GetComponent<RaycastManagerScript>().SetRaycastBlockingOnInstructionDragged();
        RaycastManagerScript.SetRaycastBlockingOnInstructionDragged();
        //CodePanel.SetRaycastBlockingForAllInstructions(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; 
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;

        CodePanel.draggedObject = null;
    }
}
