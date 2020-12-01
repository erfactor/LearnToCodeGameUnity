using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionIndicatorScript : MonoBehaviour, IPointerClickHandler
{
    GameObject directionSelector;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        directionSelector = GameObject.Find("DirectionSelector");
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
            var selectionPanel = Instantiate(directionSelector);
            selectionPanel.transform.SetParent(gameObject.transform.parent.parent);
            //direction selector is only in Instructions. gameObject.parent = Instruction, Instruction.parent = Panel. Therefore, we need gameObject.parent.parent.

            var selectionPanelScript = selectionPanel.GetComponent<DirectionSelectorScript>();
            selectionPanelScript.changedDirectionIndicator = gameObject;
        }
    }
}
