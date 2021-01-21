using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeText : MonoBehaviour
{
    CanvasGroup _canvasGroup;
    public float startingAlpha = 0.0f;
    private int frameCountToFadeCompletely = 90;

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        StopAllCoroutines();
        _canvasGroup.blocksRaycasts = true;
        StartCoroutine(CoroutineShow());
    }

    public void Hide()
    {
        StopAllCoroutines();
        _canvasGroup.blocksRaycasts = false;
        StartCoroutine(CoroutineHide());
    }

    public IEnumerator CoroutineShow()
    {
        for (int i = 0; i <= frameCountToFadeCompletely; i++) 
        {
            float calculatedAlpha = i / (float)frameCountToFadeCompletely;
            float currentAlpha = _canvasGroup.alpha;
            if (calculatedAlpha < currentAlpha) continue;
            _canvasGroup.alpha = calculatedAlpha;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public IEnumerator CoroutineHide()
    {
        for (int i = frameCountToFadeCompletely; i >= 0; i--) 
        {
            float calculatedAlpha = i / (float)frameCountToFadeCompletely;
            float currentAlpha = _canvasGroup.alpha;
            if (calculatedAlpha > currentAlpha) continue;
            _canvasGroup.alpha = calculatedAlpha;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }
}
