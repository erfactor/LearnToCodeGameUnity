using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingPanel : MonoBehaviour
{
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        canvasGroup.blocksRaycasts = true;
        StopAllCoroutines();
        StartCoroutine(CoroutineShow());
    }

    private IEnumerator CoroutineShow()
    {
        for (int i = 0; i <= 10; i++)
        {
            var newAlpha = i / 10.0f;
            if (canvasGroup.alpha > newAlpha) continue;
            canvasGroup.alpha = newAlpha;
            yield return new WaitForFixedUpdate();
        }
    }

    public void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        StopAllCoroutines();
        StartCoroutine(CoroutineHide());
    }

    private IEnumerator CoroutineHide()
    {
        for (int i = 10; i >= 0; i--)
        {
            var newAlpha = i / 10.0f;
            if (canvasGroup.alpha < newAlpha) continue;
            canvasGroup.alpha = newAlpha;
            yield return new WaitForFixedUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
