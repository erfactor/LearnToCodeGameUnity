using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Commands;

public class ComparisonSelectorScript : MonoBehaviour
{
    public GameObject changedComparisonIndicator;

    static Dictionary<ComparedType, Sprite> comparisonSprites;

    private static void InitializeComparisonSprites()
    {
        if (comparisonSprites == null)
        {
            comparisonSprites = new Dictionary<ComparedType, Sprite>();
            comparisonSprites.Add(ComparedType.Direction, Resources.Load<Sprite>("Sprites/UI/Comparison/direction"));
            comparisonSprites.Add(ComparedType.Bot, Resources.Load<Sprite>("Sprites/UI/Comparison/bot"));
            comparisonSprites.Add(ComparedType.Something, Resources.Load<Sprite>("Sprites/UI/Comparison/something"));
            comparisonSprites.Add(ComparedType.Nothing, Resources.Load<Sprite>("Sprites/UI/Comparison/nothing"));
            comparisonSprites.Add(ComparedType.Item, Resources.Load<Sprite>("Sprites/UI/DireComparisonctions/item"));
            comparisonSprites.Add(ComparedType.Number, Resources.Load<Sprite>("Sprites/UI/Comparison/number"));
            comparisonSprites.Add(ComparedType.Wall, Resources.Load<Sprite>("Sprites/UI/Comparison/wall"));
            comparisonSprites.Add(ComparedType.Hole, Resources.Load<Sprite>("Sprites/UI/Comparison/hole"));
            Debug.Log("ComparisonSelector initialization done");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComparisonSprites();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSelected(ComparedType type)
    {
        InitializeComparisonSprites();
        var newSprite = GetSprite(type);
        changedComparisonIndicator.GetComponent<Image>().sprite = newSprite;
        changedComparisonIndicator.transform.GetChild(0).name = type.ToString();
        Destroy(gameObject);
    }

    public static Sprite GetSprite(ComparedType type)
    {
        return comparisonSprites[type];
    }
}
