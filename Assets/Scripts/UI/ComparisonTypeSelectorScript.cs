using UnityEngine;
using UnityEngine.EventSystems;
using Commands;

public class ComparisonTypeSelectorScript : MonoBehaviour, IPointerClickHandler
{
    public ComparedType comparedType;

    private GameObject comparisonTypeWindow;
    private ComparisonTypeSelectionWindowScript parentScript;

    // Start is called before the first frame update
    void Start()
    {
        comparisonTypeWindow = transform.parent.gameObject;
        parentScript = comparisonTypeWindow.GetComponent<ComparisonTypeSelectionWindowScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            parentScript.SetSelected(this.comparedType);
        }
    }
}