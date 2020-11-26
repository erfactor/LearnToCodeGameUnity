using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveSelectorScript : MonoBehaviour, IPointerClickHandler
{
    public Direction direction;

    private GameObject DirectionSelector;
    private DirectionSelectorScript parentScript;

    // Start is called before the first frame update
    void Start()
    {
        DirectionSelector = transform.parent.gameObject;
        parentScript = DirectionSelector.GetComponent<DirectionSelectorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("prawy klik");

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
