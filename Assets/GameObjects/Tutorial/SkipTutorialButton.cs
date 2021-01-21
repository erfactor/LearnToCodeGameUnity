using Profiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkipTutorialButton : MonoBehaviour, IPointerClickHandler
{
    private int tutorialSkipIndex = 3;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("ProfileManager").GetComponent<ProfileManager>().MarkTutorialAsCompleted();
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(tutorialSkipIndex);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
