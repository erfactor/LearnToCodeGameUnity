using UnityEngine;
using UnityEngine.EventSystems;
using UI;

public class DragScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        canvasGroup.alpha = 0.8f;

        startPosition = transform.position;

        CodePanel.draggedObject = gameObject;
        CodePanel.draggedObject.transform.SetParent(GameObject.Find("Panel").transform);
        var codePanel = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();
        var indexOnCodePanel = codePanel.GetGameObjectIndexOnList(gameObject);
        if (indexOnCodePanel >= 0)
        {
            codePanel.unpinnedCodeline = codePanel.CurrentSolution[indexOnCodePanel];
                        
            codePanel.unpinnedCodeline.SetParent(null);

            codePanel.Unpin(codePanel.unpinnedCodeline);
            codePanel.ZipHierarchy(codePanel.unpinnedCodeline);
            codePanel.Rearrange();
        }

        //GameObject.Find("RaycastManager").GetComponent<RaycastManagerScript>().SetRaycastBlockingOnInstructionDragged();
        RaycastManagerScript.SetRaycastBlockingOnInstructionDragged();
        //CodePanel.SetRaycastBlockingForAllInstructions(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mousePos = Input.mousePosition;
        transform.position = mousePos;
        var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);// * GameObject.Find("Canvas").transform.localScale.x;
        transform.position = new Vector3(worldMousePos.x, worldMousePos.y, 100);

        Debug.DrawLine(worldMousePos, worldMousePos + new Vector3(100, 100), Color.red, 1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;

        CodePanel.draggedObject = null;
        GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayInstructionDropSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("Unhover");
    }
}
