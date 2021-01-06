using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CodePanelChangeScript : MonoBehaviour, IPointerClickHandler
{
    public Color selectedColor;
    public Color notSelectedColor;
    public int solutionIndex;
    public bool isSelected;

    private Image image;
    private Text text;

    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        if (solutionIndex == 1)
        {
            UnselectAllSiblings();
            Select();
        }
        
        text.text = solutionIndex.ToString();
    }

    public void Clicked()
    {
        Debug.Log("Clicked code panel button number " + solutionIndex);
        UnselectAllSiblings();
        Select();
    }

    public void Select()
    {
        isSelected = true;
        image.color = selectedColor;
        GameObject.Find("SolutionManager").GetComponent<SolutionManagerScript>().ChangeSolution(solutionIndex);
    }

    public void Unselect()
    {
        isSelected = false;
        image.color = notSelectedColor;
    }

    public void UnselectAllSiblings()
    {
        int buttonCount = transform.parent.childCount;

        for(int i=0; i<buttonCount; i++)
        {
            var script = transform.parent.GetChild(i).GetComponent<CodePanelChangeScript>();
            if (script == this) continue;
            script.Unselect();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked();
    }
}
