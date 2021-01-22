using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchTutorial : MonoBehaviour, IPointerClickHandler
{
    public TextAsset TutorialLevel;
    public TextAsset TutorialSolution;
    public TextAsset TutorialHint;

    public void Start()
    {
        LevelLoader.Level = new Level
        {
            File = TutorialLevel.text,
            SolutionFile = TutorialSolution.text,
            Number = 0,
            Hint = TutorialHint.text
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(5);
    }
}
