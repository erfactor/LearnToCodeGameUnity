using UnityEngine;
using UnityEngine.EventSystems;
using Commands;

public class ComparisonTypeIndicatorScript : MonoBehaviour, IPointerClickHandler
{
    GameObject comparisonTypeWindow;
    CanvasGroup canvasGroup;

    public ComparedType SelectedComparisonType { get; set; } = ComparedType.Direction;

    // Start is called before the first frame update
    void Start()
    {
        comparisonTypeWindow = GameObject.Find("ComparisonTypeSelectionWindow");
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        //Hide();
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

            var selectionPanel = Instantiate(comparisonTypeWindow);
            selectionPanel.transform.SetParent(GameObject.Find("LevelCanvas").transform);
            selectionPanel.transform.localScale = new Vector3(1, 1, 1);
            selectionPanel.transform.localPosition = new Vector3(150, 0, 0);

            var selectionPanelScript = selectionPanel.GetComponent<ComparisonTypeSelectionWindowScript>();
            selectionPanelScript.changedComparisonIndicator = gameObject;
        }
    }
}
