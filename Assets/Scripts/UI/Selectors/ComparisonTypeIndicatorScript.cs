using UnityEngine;
using UnityEngine.EventSystems;
using Commands;

public class ComparisonTypeIndicatorScript : MonoBehaviour, IPointerClickHandler
{
    GameObject comparisonTypeWindow;
    CanvasGroup canvasGroup;

    public ComparedType SelectedComparisonType { get; set; } = ComparedType.Bot;

    SelectorBlocker selectorBlocker;

    // Start is called before the first frame update
    void Start()
    {
        comparisonTypeWindow = GameObject.Find("ComparisonTypeSelectionWindow");
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        //Hide();
        selectorBlocker = GameObject.Find("SelectorBlocker").GetComponent<SelectorBlocker>();
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
            selectionPanel.transform.position = ClampPosition(transform.position);

            var selectionPanelScript = selectionPanel.GetComponent<ComparisonTypeSelectionWindowScript>();
            selectorBlocker.Show(selectionPanelScript);
            selectionPanelScript.changedComparisonIndicator = gameObject;
            selectionPanelScript.Show();
        }
    }

    public Vector3 ClampPosition(Vector3 pos)
    {
        return new Vector3(ClampXPosition(pos.x), ClampYPosition(pos.y), pos.z);
    }

    public float ClampXPosition(float xPos)
    {
        const float PopUpWindowMaxX = 870;
        return Mathf.Min(xPos, PopUpWindowMaxX);
    }

    public float ClampYPosition(float yPos)
    {
        const float PopUpWindowMaxY = 390;
        const float PopUpWindowMinY = -370;

        return Mathf.Clamp(yPos, PopUpWindowMinY, PopUpWindowMaxY);
    }
}
