using Services;
using System.Collections;
using System.Collections.Generic;
using UI.PanelButtons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(GoToMainMenu());        
    }

    public IEnumerator GoToMainMenu()
    {
        GetComponent<Animator>().SetTrigger("Hide");
        yield return new WaitForSeconds(Config.Timing.WinWindowSceneChangeDelay);
        GameObject.Find("ReturnButton").GetComponent<ReturnButtonScript>().ReturnToMainMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Show();
        }
    }

    public void Show()
    {
        var text = transform.Find("Number").GetComponent<Text>().text = GameObject.Find("LevelLoader").GetComponent<LevelLoader>().levelNumber.ToString();
        GetComponent<Animator>().SetTrigger("Show");
    }
}
