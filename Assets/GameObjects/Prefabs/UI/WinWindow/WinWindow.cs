using Services;
using System.Collections;
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

    private IEnumerator GoToMainMenu()
    {
        GetComponent<Animator>().SetTrigger("Hide");
        yield return new WaitForSeconds(Config.Timing.WinWindowSceneChangeDelay);
        GameObject.Find("ReturnButton").GetComponent<ReturnButtonScript>().ReturnToMainMenu();
    }

    public void Show()
    {
        var text = transform.Find("Number").GetComponent<Text>().text = LevelLoader.Level.Number.ToString();
        // TODO change win text
        GetComponent<Animator>().SetTrigger("Show");
    }
}
