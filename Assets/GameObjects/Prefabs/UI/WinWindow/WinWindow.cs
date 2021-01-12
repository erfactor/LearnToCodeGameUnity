using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinWindow : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(2);
        
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
        GetComponent<Animator>().SetTrigger("Show");
    }
}
