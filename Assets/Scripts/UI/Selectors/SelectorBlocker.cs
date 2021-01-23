using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectorBlocker : MonoBehaviour, IPointerClickHandler
{
    CanvasGroup _canvasGroup;
    public ISelectionWindow windowToClose;
    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void Show(ISelectionWindow windowToClose)
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        this.windowToClose = windowToClose;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (windowToClose == null) return;
        windowToClose.Hide();
        Hide();
    }

}
