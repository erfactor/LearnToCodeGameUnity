using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Commands;

public class ExecutionIndicatorManager : MonoBehaviour
{
    LevelLoader levelLoader;
    Dictionary<Bot, GameObject> indicators = new Dictionary<Bot, GameObject>();
    Dictionary<Bot, int> indicatorPositions = new Dictionary<Bot, int>();
    public Dictionary<ICommand, CodeLine> commandToCodeLineMappings;

    public GameObject indicatorPrefab;

    CodePanel panel;

    public List<Color> indicatorColors;

    public const int YSpacing = 6;

    public const int startingPosition = 1;

    void Start()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    public void AssignColorsToBots(List<Bot> bots)
    {
        for (int i = 0; i < bots.Count; i++)
        {
            bots[i].Color = indicatorColors[i];
        }
    }

    public void InstantiateIndicators(List<Bot> bots)
    {
        panel = GameObject.Find("SolutionPanel").GetComponent<CodePanel>();

        foreach (var bot in bots)
        {
            if (indicators.ContainsKey(bot)) continue;
            var indicator = Instantiate(GameObject.Find("ExecutionIndicator"), panel.transform.Find("RootContainer"), true);
            indicator.GetComponent<Image>().color = bot.Color;
            indicators.Add(bot, indicator);
            indicatorPositions.Add(bot, startingPosition);
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
        if (!indicators.ContainsKey(bot)) return;
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

    

    void MoveTowards(GameObject indicator, Vector2 destination)
    {
        indicator.transform.position = Vector2.Lerp(indicator.transform.position, destination, 0.5f);
    }    

    public void UpdateIndicator(Bot bot, int commandId, ICommand command)
    {
        indicatorPositions[bot] = commandId;
        var indicatorToUpdate = indicators[bot];
        var codeLine = commandToCodeLineMappings[command];
        indicatorToUpdate.transform.SetParent(codeLine.instruction.transform, true);
        StartCoroutine(CoroutineMoveToPosition(indicatorToUpdate, codeLine.instruction, commandId, bot));
        SortIndicators();
    }

    public IEnumerator CoroutineMoveToPosition(GameObject indicator, GameObject instruction, int commandId, Bot bot)
    {
        for (int i = 0; i < 20; i++)
        {
            var desiredPosition = GetPositionForIndicator(instruction, commandId, bot);
            MoveTowards(indicator, desiredPosition);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }
    Vector2 GetPositionForIndicator(GameObject instruction, int commandId, Bot bot)
    {
        Vector2 indicatorOffset = new Vector2(-instruction.GetComponent<RectTransform>().sizeDelta.x / 2 - 10, 0);
        Vector2 instructionPosition = instruction.transform.position;
        return instructionPosition + indicatorOffset + GetIndicatorYOffset(commandId, bot);
    }

    public Vector2 GetIndicatorYOffset(int commandId, Bot bot)
    {
        var botsOnThisId = indicatorPositions.Where(x => x.Value == commandId).Select(x => x.Key).ToList();
        var index = botsOnThisId.IndexOf(bot);
        switch (index)
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
