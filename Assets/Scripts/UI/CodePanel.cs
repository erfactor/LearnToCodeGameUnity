using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodePanel : MonoBehaviour, IDropHandler
{

    public const float SpacingY = 10; // height of the space between two lines
    public const float SpacingX = 20; // left Margin

    public const float lineSizeX = 150;
    public const float lineSizeY = 40; //height of the single line

    public static GameObject draggedObject;

    private List<CodeLine> codeLines;
    private float ScrollY;

    private Vector2 topLeftCorner;
    private Rect panelRect;

    // Start is called before the first frame update
    void Start()
    {
        codeLines = new List<CodeLine>();
        draggedObject = null;
        ScrollY = 0;

        SetHelper();
    }

    void SetHelper()
    {
        Rect rect = this.gameObject.GetComponent<RectTransform>().rect;
        float left = gameObject.transform.position.x - rect.width / 2;
        float top = gameObject.transform.position.y + rect.height / 2;
        topLeftCorner = new Vector2(left, top);

        panelRect = new Rect(left, top, rect.width, rect.height);
    }

    public bool IsInRect(Rect r, Vector2 pos)
    {
        return (pos.x >= r.x && pos.x <= r.x + r.width && pos.y <= r.y && pos.y >= r.y - r.height);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (draggedObject != null && IsInRect(panelRect, Input.mousePosition))
        {
            HandleDrag();
        }

        UpdateScroll();
        SetPositions();
    }

    public void HandleDrag()
    {

    }

    public void UpdateScroll()
    {

    }

    public void SetPositions()
    {
        bool isDragged = draggedObject != null && IsInRect(panelRect, Input.mousePosition);
        //if (isDragged) Debug.Log("Set positions, isDragged");

        for (int i = 0; i < codeLines.Count; i++)
        {
            bool isThisOneDragged = draggedObject == codeLines[i].go;
            codeLines[i].UpdatePosition(isDragged, isThisOneDragged);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop, codepanel");
        if (eventData.pointerDrag != null)
        {
            HandleDrop(eventData);
        }
    }

    private void HandleDrop(PointerEventData eventData)
    {
        int index = GetSlotIndexUnderMousePosition(eventData);

        InsertAtSlot(index, eventData.pointerDrag); //TODO: sprawdzic czy tutaj nie lepiej dac selectedObject
    }

    private int GetSlotIndexUnderMousePosition(PointerEventData eventData)
    {
        if (codeLines.Count == 0) return 0;

        else
        {
            if (eventData.position.y >= codeLines[0].dockPosition.y) // add at the top
            {
                return 0;
            }

            else if (eventData.position.y < codeLines[codeLines.Count - 1].dockPosition.y)//add at the bottom
            {
                return codeLines.Count;
            }

            else
            {
                for (int i = 0; i < codeLines.Count - 1; i++)
                {
                    if (eventData.position.y <= codeLines[i].dockPosition.y && eventData.position.y > codeLines[i + 1].dockPosition.y)
                    {
                        return i + 1;
                    }
                }
            }
        }

        throw new System.Exception("Could not find a slot to fill.");
    }

    public int GetGameObjectIndexOnList(GameObject go)
    {
        for (int i = 0; i < codeLines.Count; i++)
        {
            if (codeLines[i].go == go) return i;
        }

        return -1;
    }

    private void InsertAtSlot(int index, GameObject go)
    {
        int indexOfPresentLine = GetGameObjectIndexOnList(go); //check if the dragged object is already on the code panel

        if (indexOfPresentLine >= 0)
        {
            CodeLine draggedLine = codeLines[indexOfPresentLine];
            codeLines.RemoveAt(indexOfPresentLine);

            if (index > indexOfPresentLine) index--;
            codeLines.Insert(index, draggedLine);
        }
        
        else
        {
            Vector2 newDockPosition = GetAbsoluteDockPositionForIndex(index);
            CodeLine newCodeLine = new CodeLine(go, newDockPosition, index == 0, index == codeLines.Count);
            codeLines.Insert(index, newCodeLine);
        }

        //Vector2 newDockPosition = GetAbsoluteDockPositionForIndex(index);
        //CodeLine newCodeLine = new CodeLine(go, newDockPosition, index == 0, index == codeLines.Count);
        //codeLines.Insert(index, newCodeLine);

        Rearrange();
    }

    private void Rearrange()
    {
        for (int i = 0; i < codeLines.Count; i++)
        {
            codeLines[i].SetTopBot(i == 0, i == codeLines.Count - 1);
            codeLines[i].ChangeDockPosition(GetAbsoluteDockPositionForIndex(i));
            //codeLines[i] = new CodeLine(codeLines[i].go, GetAbsoluteDockPositionForIndex(i), i == 0, i == codeLines.Count - 1);
        }
    }

    private Vector2 GetAbsoluteDockPositionForIndex(int index)
    {
        float newY = topLeftCorner.y - index * lineSizeY - (index + 1) * SpacingY - lineSizeY / 2;
        float newX = topLeftCorner.x + SpacingX + lineSizeX / 2;
        return new Vector2(newX, newY);
    }
}

public class CodeLine
{
    public GameObject go;
    public const float SizeY = 40; //height of the single line
    public const float SpacingY = 10; // height of the space between two lines

    public float lockCageTopY;
    public float lockCageBotY;

    public float lockCageSize => SizeY + SpacingY;

    public Vector2 dockPosition;

    private Vector2 velocity;

    public CodeLine(GameObject go, Vector2 dockPosition, bool isTop, bool isBottom)
    {
        this.velocity = Vector2.zero;

        this.go = go;
        this.dockPosition = dockPosition;
        go.transform.position = dockPosition;

        SetTopBot(isTop, isBottom);
    }

    public void ChangeDockPosition(Vector2 newDockPosition, int indent = 0)
    {
        this.dockPosition = newDockPosition;
        lockCageTopY = dockPosition.y + lockCageSize;
        lockCageBotY = dockPosition.y - lockCageSize;
    }

    public void SetTopBot(bool isTop, bool isBottom)
    {
        lockCageTopY = dockPosition.y + lockCageSize;
        lockCageBotY = dockPosition.y - lockCageSize;

        if (isTop)
        {
            //lockCageTopY = dockPosition.y;
        }

        else if (isBottom)
        {
            //lockCageBotY = dockPosition.y;
        }
    }

    public const float MinDistanceForRepelForce = 10f;
    public const float MaxDistanceForRepelForce = SizeY + SpacingY; // height of the single element
    public const float ReturnForceScaleFactor = 0.08f;
    public const float RepelForceScaleFactor = 8;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="forceSourcePosition">Mouse position AFTER applying the scroll</param>
    public Vector2 GetRepelForce(Vector2 forceSourcePosition)
    {
        Vector2 pos2 = dockPosition;

        float yDiff = dockPosition.y - forceSourcePosition.y;

        //if (Mathf.Abs(yDiff) > MaxDistanceForRepelForce) return Vector2.zero;
        yDiff = Mathf.Clamp(yDiff, -MinDistanceForRepelForce, MinDistanceForRepelForce);

        if (yDiff >= 0 && yDiff < MinDistanceForRepelForce) yDiff = MinDistanceForRepelForce;
        if (yDiff <= 0 && yDiff > -MinDistanceForRepelForce) yDiff = -MinDistanceForRepelForce;

        //return new Vector2(0, RepelForceScaleFactor / Mathf.Sqrt(Mathf.Abs(yDiff)) * Mathf.Sign(yDiff));
        return new Vector2(0, RepelForceScaleFactor / Mathf.Pow(Mathf.Abs(yDiff), 0.6f) * Mathf.Sign(yDiff));
    }

    public Vector2 GetReturnVector()
    {
        //return Vector2.zero;

        float yDiff = dockPosition.y - go.transform.position.y;

        if (yDiff == 0) return Vector2.zero;
        if (yDiff >= 0 && yDiff < 0.1) yDiff = 0.1f;
        if (yDiff <= 0 && yDiff > -0.1) yDiff = -0.1f;

        Vector2 newForce = new Vector2(0, ReturnForceScaleFactor * yDiff);
        //Debug.Log($"yDiff at repel: {yDiff}");
        return newForce;
    }

    public void CheckIfOutOfBounds()
    {
        if (this.go.transform.position.y > lockCageTopY)
        {
            this.go.transform.position = new Vector2(this.go.transform.position.x, lockCageTopY);
        }

        if (this.go.transform.position.y < lockCageBotY)
        {
            this.go.transform.position = new Vector2(this.go.transform.position.x, lockCageBotY);
        }
    }

    public void UpdatePosition(bool isAnythingDragged, bool isThisOneDragged)
    {
        if (isThisOneDragged) return;

        Vector2 sumForce;// = GetRepelForce(Input.mousePosition) + GetReturnVector();

        if (isAnythingDragged)
        {
            sumForce = GetRepelForce(Input.mousePosition) + GetReturnVector();
        }
        else
        {
            sumForce = GetReturnVector();
        }

        velocity = sumForce;

        //Vector2 newPosition = new Vector2(go.transform.position.x + velocity.x, go.transform.position.y + velocity.y);
        Vector2 newPosition = new Vector2(dockPosition.x, go.transform.position.y + velocity.y);
        go.transform.position = newPosition;
        CheckIfOutOfBounds();
    }
}
