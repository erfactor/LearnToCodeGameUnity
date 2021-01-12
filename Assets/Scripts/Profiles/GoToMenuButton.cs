using Profiles;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GoToMenuButton : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();
        var profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        this.GetComponent<Button>().onClick.AddListener(ProceedToMenu);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ProceedToMenu()
    {
        StartCoroutine(CoroutineProceedToMenu());
    }

    public IEnumerator CoroutineProceedToMenu()
    {
        yield return CoroutineAscendProfile();
        var profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        profileManager.GoToMainMenu(transform.parent.gameObject);
        yield break;
    }

    public IEnumerator CoroutineAscendProfile()
    {
        var rectTransform = transform.parent.GetComponent<RectTransform>();
        var currentPosition = rectTransform.anchoredPosition;
        Vector2 destination = new Vector2(currentPosition.x, -2000);

        for (int i = 0; i < 30; i++)
        {
            currentPosition = Vector2.Lerp(currentPosition, destination, 0.001f + i * 0.0006f);
            Debug.Log($"currentSize: {currentPosition}");
            rectTransform.anchoredPosition = currentPosition;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }
}
