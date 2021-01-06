using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionIndicatorScript : MonoBehaviour, IPointerClickHandler
{
    GameObject directionSelectionWindow;
    CanvasGroup canvasGroup;

    public Direction SelectedDirection { get; set; } = Direction.Right;

    // Start is called before the first frame update
    void Start()
    {
        directionSelectionWindow = GameObject.Find("DirectionSelectionWindow");
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RaycastManagerScript.SetFocus();

            var selectionPanel = Instantiate(directionSelectionWindow);
            selectionPanel.transform.SetParent(GameObject.Find("LevelCanvas").transform);
            selectionPanel.transform.localScale = new Vector3(1, 1, 1);
            selectionPanel.transform.localPosition = new Vector3(200, 0, 0);

            var selectionPanelScript = selectionPanel.GetComponent<DirectionSelectionWindowScript>();
            selectionPanelScript.changedDirectionIndicator = gameObject;
        }
    }
}
