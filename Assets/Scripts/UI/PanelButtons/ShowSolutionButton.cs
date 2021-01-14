using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowSolutionButton : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void ShowSolution()
    {
        GameObject.Find("SolutionWindow").GetComponent<SolutionWindow>().Show();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowSolution();
    }
}
