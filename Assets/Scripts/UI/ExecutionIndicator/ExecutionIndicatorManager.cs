﻿using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ExecutionIndicatorManager : MonoBehaviour
{
    LevelLoader levelLoader;
    Dictionary<Bot, GameObject> indicators = new Dictionary<Bot, GameObject>();
    Dictionary<Bot, int> indicatorPositions = new Dictionary<Bot, int>();
    //List<Bot> bots = new List<Bot>();

    public GameObject indicatorPrefab;

    CodePanel panel;

    public List<Color> indicatorColors;

    public const int YSpacing = 6;

    public const int startingPosition = 1;

    void Start()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    public void InstantiateIndicators(List<Bot> bots)
    {
        panel = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();

        for (int i=0; i<bots.Count; i++)
        {
            if (indicators.ContainsKey(bots[i])) continue;
            var indicator = Instantiate(GameObject.Find("ExecutionIndicator"));
            indicator.transform.SetParent(panel.transform);
            indicator.GetComponent<Image>().color = indicatorColors[i];
            bots[i].Animator.gameObject.transform.Find("Body copy").Find("ColorIndicator").GetComponent<SpriteRenderer>().color = indicatorColors[i];
            indicators.Add(bots[i], indicator);
            indicatorPositions.Add(bots[i], startingPosition);
        }
    }
    // Update is called once per frame
    void Update()
    {      
        if (levelLoader.ShouldDisplayExecutionIndicators())
        {
            panel = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();
            SortIndicators();
        }
        else
        {            
            //ClearIndicators();            
        }
    }

    void SortIndicators()
    {
        List<GameObject> sortedIndicators = indicators.Values.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList();
        for(int i=0; i<sortedIndicators.Count; i++)
        {
            sortedIndicators[i].transform.SetSiblingIndex(panel.transform.childCount - 1 - i);
        }
    }

    public void RemoveIndicatorForBot(Bot bot)
    {
        var indicator = indicators[bot];
        indicators.Remove(bot);
        indicatorPositions.Remove(bot);
        StartCoroutine(CoroutineMakeIndicatorDissapear(indicator));        
    }

    public IEnumerator CoroutineMakeIndicatorDissapear(GameObject indicator)
    {
        var rectTransform = indicator.GetComponent<RectTransform>();
        for (int i = 0; i < 30; i++)
        {
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, Vector2.zero, 0.2f);
            yield return new WaitForFixedUpdate();
        }

        Destroy(indicator);
    }

    public void ClearIndicators()
    {
        //StopAllCoroutines();

        var bots = new List<Bot>();
        bots.AddRange(indicators.Keys);

        foreach(var bot in bots)
        {
            RemoveIndicatorForBot(bot);
        }
    }

    Vector2 GetPositionForIndicator(GameObject instruction, int commandId)
    {
        Vector2 indicatorOffset = new Vector2(-instruction.GetComponent<RectTransform>().sizeDelta.x/2 - 10, 0);
        Vector2 instructionPosition = instruction.GetComponent<RectTransform>().anchoredPosition;
        return instructionPosition + indicatorOffset + GetIndicatorYOffset(commandId);
    }

    void MoveTowards(GameObject indicator, Vector2 destination)
    {
        var rectTransform = indicator.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, destination, 0.33f);
    }

    public void UpdateIndicator(Bot bot, int commandId)
    {
        indicatorPositions[bot] = commandId;
        var indicatorToUpdate = indicators[bot];
        var instruction = panel.CurrentSolution[commandId];
        StartCoroutine(CoroutineMoveToPosition(indicatorToUpdate, GetPositionForIndicator(instruction.go, commandId)));
        SortIndicators();
    }

    public IEnumerator CoroutineMoveToPosition(GameObject indicator, Vector2 destination)
    {
        for (int i = 0; i < 30; i++)
        {
            MoveTowards(indicator, destination);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public Vector2 GetIndicatorYOffset(int commandId)
    {
        int stackSize = indicatorPositions.Where(x => x.Value == commandId).Count() - 1;
        switch (stackSize)
        {
            case 0: return new Vector2(0, 0);
            case 1: return new Vector2(0, -YSpacing);
            case 2: return new Vector2(0, +YSpacing);
            case 3: return new Vector2(0, -2 * YSpacing);
            case 4: return new Vector2(0, 2 * YSpacing);
            case 5: return new Vector2(0, -3 * YSpacing);
            case 6: return new Vector2(0, 3 * YSpacing);
            case 7: return new Vector2(0, -4 * YSpacing);
            case 8: return new Vector2(0, 4 * YSpacing);
            default: return new Vector2(0, 0);
        }
    }
}
