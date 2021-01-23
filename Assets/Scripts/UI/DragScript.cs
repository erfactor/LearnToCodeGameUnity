using UnityEngine;
using UnityEngine.EventSystems;
using UI;

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
        newObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsReplicator)
        {
            IsReplicator = false;
            Replicate();
            transform.SetParent(transform.parent.parent);
        }

        GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayInstructionGrabSound();

        Debug.Log("OnBeginDrag");
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        startPosition = transform.position;

        CodePanel.draggedObject = gameObject;
        CodePanel.draggedObject.transform.SetParent(GameObject.Find("UIPanel").transform);
        var codePanel = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();
        var codeLineOnCodePanel = codePanel.GetCorrespondingCodeLine(gameObject);

        if (codeLineOnCodePanel != null)
        {
            eventData.pointerDrag = codeLineOnCodePanel.container;
            codePanel.Unpin(codeLineOnCodePanel);
        }

        RaycastManagerScript.SetRaycastBlockingOnInstructionDragged();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mousePos = Input.mousePosition;
        transform.position = mousePos;
        var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var codePanelScript = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();
        var offset = codePanelScript.CalculateOffsetForHeldContainer(eventData.pointerDrag);
        //Debug.Log($"offset: {offset}, name: {eventData.pointerDrag.name}", eventData.pointerDrag);
        transform.position = new Vector3(worldMousePos.x, worldMousePos.y, 100) - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;

        CodePanel.draggedObject = null;
        GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayInstructionDropSound();
        RaycastManagerScript.SetRaycastBlockingAfterInstructionReleased();
    }
}
