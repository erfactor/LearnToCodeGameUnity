using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComparisonIndicatorScript : MonoBehaviour, IPointerClickHandler
{
    GameObject comparisonTypeSelector;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        comparisonTypeSelector = GameObject.Find("ComparisonTypeSelector");
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
            var selectionPanel = Instantiate(comparisonTypeSelector);
            selectionPanel.transform.SetParent(gameObject.transform.parent.parent);
            //direction selector is only in Instructions. gameObject.parent = Instruction, Instruction.parent = Panel. Therefore, we need gameObject.parent.parent.

            var selectionPanelScript = selectionPanel.GetComponent<ComparisonSelectorScript>();
            selectionPanelScript.changedComparisonIndicator = gameObject;
        }
    }
}
