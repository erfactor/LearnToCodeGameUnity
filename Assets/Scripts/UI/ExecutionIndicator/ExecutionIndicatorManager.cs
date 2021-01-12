using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExecutionIndicatorManager : MonoBehaviour
{
    LevelLoader levelLoader;
    Dictionary<Bot, GameObject> indicators = new Dictionary<Bot, GameObject>();
    //List<Bot> bots = new List<Bot>();

    public GameObject indicatorPrefab;

    CodePanel panel;

    List<Color> indicatorColors = new List<Color>()
    {
        Color.green,
        Color.red,
        Color.blue,
        Color.yellow,
        Color.white,
        Color.magenta,
        Color.cyan,
    };

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
            indicators.Add(bots[i], indicator);
        }
    }
    // Update is called once per frame
    void Update()
    {      
        if (levelLoader.ShouldDisplayExecutionIndicators())
        {
            panel = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();
        }
        else
        {
            //ClearIndicators();            
        }
    }

    public void RemoveIndicatorForBot(Bot bot)
    {
        var indicator = indicators[bot];
        indicators.Remove(bot);
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

    Vector2 GetPositionForIndicator(GameObject instruction)
    {
        Vector2 indicatorOffset = new Vector2(-instruction.GetComponent<RectTransform>().sizeDelta.x/2 - 10, 0);
        Vector2 instructionPosition = instruction.GetComponent<RectTransform>().anchoredPosition;
        return instructionPosition + indicatorOffset;
    }

    void MoveTowards(GameObject indicator, Vector2 destination)
    {
        var rectTransform = indicator.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, destination, 0.33f);
    }

    public void UpdateIndicator(Bot bot, int commandId)
    {
        var indicatorToUpdate = indicators[bot];
        var instruction = panel.CurrentSolution[commandId];
        StartCoroutine(CoroutineMoveToPosition(indicatorToUpdate, GetPositionForIndicator(instruction.go)));
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
}
