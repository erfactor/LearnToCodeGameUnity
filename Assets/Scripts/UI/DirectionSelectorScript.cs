using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionSelectorScript : MonoBehaviour, IPointerClickHandler
{
    public Direction direction;

    private GameObject DirectionSelector;
    private DirectionSelectionWindowScript parentScript;

    // Start is called before the first frame update
    void Start()
    {
        DirectionSelector = transform.parent.gameObject;
        parentScript = DirectionSelector.GetComponent<DirectionSelectionWindowScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            parentScript.SetSelected(this.direction);
        }
    }
}

public enum Direction
{
    UpLeft = 0,
    Up = 1,
    UpRight = 2,
    Right = 3,
    DownRight = 4,
    Down = 5,
    DownLeft = 6,
    Left = 7,
    Center = 8,
}
