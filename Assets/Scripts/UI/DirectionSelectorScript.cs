using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionSelectorScript : MonoBehaviour
{
    public GameObject changedDirectionIndicator;

    static Dictionary<Direction, Sprite> directionSprites;

    private static void InitializeDirectionSprites()
    {
        if (directionSprites == null)
        {
            directionSprites = new Dictionary<Direction, Sprite>();
            directionSprites.Add(Direction.UpRight, Resources.Load<Sprite>("Sprites/UI/Directions/direction_upright"));
            directionSprites.Add(Direction.Right, Resources.Load<Sprite>("Sprites/UI/Directions/direction_right"));
            directionSprites.Add(Direction.DownRight, Resources.Load<Sprite>("Sprites/UI/Directions/direction_downright"));
            directionSprites.Add(Direction.Left, Resources.Load<Sprite>("Sprites/UI/Directions/direction_left"));
            directionSprites.Add(Direction.UpLeft, Resources.Load<Sprite>("Sprites/UI/Directions/direction_upleft"));
            directionSprites.Add(Direction.DownLeft, Resources.Load<Sprite>("Sprites/UI/Directions/direction_downleft"));
            directionSprites.Add(Direction.Up, Resources.Load<Sprite>("Sprites/UI/Directions/direction_up"));
            directionSprites.Add(Direction.Center, Resources.Load<Sprite>("Sprites/UI/Directions/direction_center"));
            directionSprites.Add(Direction.Down, Resources.Load<Sprite>("Sprites/UI/Directions/direction_down"));
            Debug.Log("DirectionSelector initialization done");
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeDirectionSprites();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected(Direction direction)
    {
        InitializeDirectionSprites();
        var newSprite = GetSprite(direction);
        changedDirectionIndicator.GetComponent<Image>().sprite = newSprite;
        changedDirectionIndicator.transform.GetChild(0).name = direction.ToString();
        Destroy(gameObject);
    }

    public static Sprite GetSprite(Direction direction)
    {
        return directionSprites[direction];
    }
}
