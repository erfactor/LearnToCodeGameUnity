using Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodePanel : MonoBehaviour, IDropHandler, IScrollHandler
{

    public float SpacingY = 40; // height of the space between two lines
    public float SpacingX = 20; // left Margin

    public float MarginTop => 40;
    public float MarginLeft => 40;

    private float lineSizeX = 150;
    private float lineSizeY = 40; //height of the single line

    private float ScrollMultiplier = 5;

    public static GameObject draggedObject;


    private List<List<CodeLine>> solutions;
    private const int numberOfSolutions = 3;
    private static int currentSolutionIndex = 0;

    private List<CodeLine> CurrentSolution => solutions[currentSolutionIndex];

    private float scrollY;

    private float CanvasScale => GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;

    private static Vector2 topLeftCorner;
    private static Rect panelRect;

    private void InitializeSolutions()
    {
        solutions = new List<List<CodeLine>>();
        for(int i=0; i<numberOfSolutions; i++)
        {
            solutions.Add(new List<CodeLine>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeSolutions();
        InitializePanel();
        InitializeConstants();

        draggedObject = null;
        scrollY = 0;
    }

    void InitializeConstants()
    {
        lineSizeX = 0;
        lineSizeY = 0;

        Rect rect = GameObject.Find("MoveInstruction").GetComponent<RectTransform>().rect;
        lineSizeX = rect.width * CanvasScale;
        lineSizeY = rect.height * CanvasScale;

        SpacingX = 20;
        SpacingY = lineSizeY / 4;
    }

    void InitializePanel()
    {
        Rect rect = this.gameObject.GetComponent<RectTransform>().rect;

        float left = gameObject.transform.position.x - rect.width / 2 * CanvasScale;
        float top = gameObject.transform.position.y + rect.height / 2 * CanvasScale;

        topLeftCorner = new Vector2(left, top);

        Debug.Log($"InitializePanel: x: {left} y :{top}");

        panelRect = new Rect(left, top, rect.width, rect.height);
    }

    public static bool IsInRect(Rect r, Vector2 pos)
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

    public void ChangeCurrentSolution(int newIndex)
    {
        currentSolutionIndex = newIndex;
    }

    public void HandleDrag()
    {

    }

    public void UpdateScroll()
    {

    }

    private bool IsMouseInCodePanel()
    {
        var absoluteRect = gameObject.GetComponent<RectTransform>().rect; 
        Rect relativeRect = new Rect(-absoluteRect.width/2, absoluteRect.height/2, absoluteRect.width, absoluteRect.height);
        Vector2 position = GetMousePositionOnCodePanel();
        bool isIn = IsInRect(relativeRect, position);

        Debug.Log(position);

        if (isIn)
        {
            ;
        }

        return isIn;
    }

    public void SetPositions()
    {
        bool isAnythingDragged = draggedObject != null && IsMouseInCodePanel();
        //if (isDragged) Debug.Log("Set positions, isDragged");

        Vector2 relativeMousePosition = GetMousePositionOnCodePanel();

        for (int i = 0; i < CurrentSolution.Count; i++)
        {
            bool isThisOneDragged = draggedObject == CurrentSolution[i].go;
            CurrentSolution[i].UpdatePosition(isAnythingDragged, isThisOneDragged, scrollY, relativeMousePosition);
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
        ShowDirectionIndicatorIfNeeded(eventData);
        RaycastManagerScript.SetRaycastBlockingAfterInstructionReleased();

        int index = GetSlotIndexUnderMousePosition(eventData);

        InsertAtSlot(index, eventData.pointerDrag); //TODO: sprawdzic czy tutaj nie lepiej dac selectedObject
    }

    private static void ShowDirectionIndicatorIfNeeded(PointerEventData eventData)
    {
        if (eventData.pointerDrag.transform.Find("DirectionIndicator") != null)
        {
            eventData.pointerDrag.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>().Show();
        }
    }

    private Vector2 TranslateMousePositionToPanel(Vector2 position)
    {
        //Debug.LogWarning("NEW CALCULATION");
        //Vector2 panelTopLeftCorner = topLeftCorner;
        //Debug.Log($"topleft: {panelTopLeftCorner}");
        //var offsetFromTopLeft = position - panelTopLeftCorner;
        //Debug.Log($"offset: {offsetFromTopLeft}");
        //var panelRect = gameObject.GetComponent<RectTransform>().rect;
        //Vector2 middle = new Vector2(panelRect.width / 2, panelRect.height / 2);
        //Debug.Log($"Middle: {middle}");
        //Vector2 distanceFromMiddle = offsetFromTopLeft - middle;
        //var transformed = distanceFromMiddle;
        //Debug.Log($"transformed: {transformed}");
        //return transformed;

        var middle = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
       // Debug.Log($"Middle: {middle}");
        var transformed = position - middle;
        //Debug.Log($"transformed: {transformed}");
        return transformed;
    }

    private Vector2 GetMousePositionOnCodePanel()
    {
        Vector2 absoluteMousePosition = Input.mousePosition;
        Vector2 worldSpaceCoordinates = Camera.main.ScreenToWorldPoint(absoluteMousePosition);
        Vector2 afterScrollAdding = worldSpaceCoordinates + new Vector2(0, scrollY);
        Vector2 mousePositionOnCodePanel = TranslateMousePositionToPanel(afterScrollAdding);
        return mousePositionOnCodePanel;
    }

    private int GetSlotIndexUnderMousePosition(PointerEventData eventData)
    {
        eventData.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        eventData.position += new Vector2(0, scrollY);
        eventData.position = TranslateMousePositionToPanel(eventData.position);

        Debug.Log($"After adjustment {eventData.position}");

        if (CurrentSolution.Count == 0) return 0;

        else
        {
            if (eventData.position.y >= CurrentSolution[0].dockPosition.y) // add at the top
            {
                return 0;
            }

            else if (eventData.position.y < CurrentSolution[CurrentSolution.Count - 1].dockPosition.y)//add at the bottom
            {
                return CurrentSolution.Count;
            }

            else
            {
                for (int i = 0; i < CurrentSolution.Count - 1; i++)
                {
                    if (eventData.position.y <= CurrentSolution[i].dockPosition.y && eventData.position.y > CurrentSolution[i + 1].dockPosition.y)
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
        for (int i = 0; i < CurrentSolution.Count; i++)
        {
            if (CurrentSolution[i].go == go) return i;
        }

        return -1;
    }

    private void InsertAtSlot(int index, GameObject go)
    {
        int indexOfPresentLine = GetGameObjectIndexOnList(go); //check if the dragged object is already on the code panel

        if (indexOfPresentLine >= 0)
        {
            CodeLine draggedLine = CurrentSolution[indexOfPresentLine];
            CurrentSolution.RemoveAt(indexOfPresentLine);

            if (index > indexOfPresentLine) index--;
            CurrentSolution.Insert(index, draggedLine);
        }
        
        else
        {
            InsertNormalInstruction(index, go);
        }

        if (InstructionHelper.IsJumpInstruction(go) && indexOfPresentLine < 0)
        {
            InsertJumpInstruction(index, go);
        }

        Rearrange();
    }

    public void InsertNormalInstruction(int index, GameObject go)
    {
        Vector2 newDockPosition = GetAbsoluteDockPositionForIndex(index);
        CodeLine newCodeLine = new CodeLine(go, newDockPosition, index == 0, index == CurrentSolution.Count);
        CurrentSolution.Insert(index, newCodeLine);
        go.transform.SetParent(transform);
    }

    public void InsertJumpInstruction(int index, GameObject go)
    {
        GameObject jumpInstructionLabel = GetJumpInstructionLabel(go);
        Vector2 newDockPosition = GetAbsoluteDockPositionForIndex(index);
        CodeLine newCodeLine = new CodeLine(jumpInstructionLabel, newDockPosition, index == 0, index == CurrentSolution.Count);
        CurrentSolution.Insert(index, newCodeLine);
        go.GetComponent<JumpInstructionScript>().AttachArrow();
    }

    public void Remove(GameObject go)
    {
        if (InstructionHelper.IsJumpInstruction(go))
        {
            GameObject bindedInstruction = go.GetComponent<JumpInstructionScript>().bindedInstruction;
            if (bindedInstruction != null)
            {
                go.GetComponent<JumpInstructionScript>().bindedInstruction = null;
                Remove(bindedInstruction);
            }
        }

        int indexOfLineToRemove = GetGameObjectIndexOnList(go);        

        if (indexOfLineToRemove >= 0)
        {
            CurrentSolution.RemoveAt(indexOfLineToRemove);
            Destroy(go);
            Rearrange();
        }
    }

    private void Rearrange()
    {
        for (int i = 0; i < CurrentSolution.Count; i++)
        {
            CurrentSolution[i].SetTopBot(i == 0, i == CurrentSolution.Count - 1);
            CurrentSolution[i].ChangeDockPosition(GetAbsoluteDockPositionForIndex(i));
        }
    }

    private Vector2 GetAbsoluteDockPositionForIndex(int index)
    {
        var calculatedDockPosition = GetAbsoluteDockPositionForFirstBlock();

        for(int i=0; i<index; i++)
        {
            var nextBlockSize = CurrentSolution[i].BlockSize.y;
            calculatedDockPosition.y -= nextBlockSize;
        }

        return calculatedDockPosition;
    }

    private Vector2 GetAbsoluteDockPositionForFirstBlock()
    {
        float xPos = MarginLeft;
        float yPos = MarginTop;

        float panelHeight = gameObject.GetComponent<RectTransform>().rect.height;

        return new Vector2(xPos, panelHeight - yPos);
    }

    public void SetRaycastBlockingForAllInstructions(bool blockRaycasts)
    {
        foreach (var instruction in CurrentSolution)
        {
            instruction.go.GetComponent<CanvasGroup>().blocksRaycasts = blockRaycasts;
        }
    }

    public GameObject GetJumpInstructionLabel(GameObject jumpInstruction)
    {
        GameObject jumpInstructionLabel = jumpInstruction.GetComponent<JumpInstructionScript>().CreateBindedLabel();
        jumpInstructionLabel.transform.SetParent(transform);
        jumpInstructionLabel.transform.localScale = new Vector3(1, 1, 1);
        return jumpInstructionLabel;
    }
    
    public List<ICommand> GetCommands()
    {
        var commandHelper = new CommandHelper();
        return commandHelper.GetCommands(CurrentSolution);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ApplyScroll(eventData);
    }

    private void ApplyScroll(PointerEventData eventData)
    {
        this.scrollY += eventData.scrollDelta.y * ScrollMultiplier;
        Debug.Log($"Added {eventData.scrollDelta.y} to scrollY, now scrollY = {scrollY}");
        TrimScroll();
    }

    private void TrimScroll()
    {
        scrollY = Mathf.Min(0, scrollY);
    }
}

public class CommandHelper
{
    public ICommand GetCommand(List<CodeLine> solution, int currentLine)
    {
        var line = solution[currentLine];

        if (InstructionHelper.IsMoveInstruction(line.go))
        {
            return new MoveCommand(InstructionHelper.GetInstructionDirection(line.go), currentLine + 1);
        }

        if (InstructionHelper.IsPickInstruction(line.go))
        {
            return new PickCommand(currentLine + 1);
        }

        if (InstructionHelper.IsPutInstruction(line.go))
        {
            return new PutCommand(currentLine + 1);
        }

        if (InstructionHelper.IsAddInstruction(line.go))
        {
            return new AddCommand(currentLine + 1);
        }

        if (InstructionHelper.IsSubInstruction(line.go))
        {
            return new SubCommand(currentLine + 1);
        }

        if (InstructionHelper.IsDecInstruction(line.go))
        {
            return new DecCommand(currentLine + 1);
        }

        if (InstructionHelper.IsIncInstruction(line.go))
        {
            return new IncCommand(currentLine + 1);
        }

        if (InstructionHelper.IsJumpInstruction(line.go))
        {
            if (InstructionHelper.IsJumpInstructionLabel(line.go))
            {
                return new JumpCommand(currentLine + 1);
            }

            int jumpIndex = GetJumpLineNumber(solution, line.go);
            return new JumpCommand(jumpIndex);
        }

        if (InstructionHelper.IsIfInstruction(line.go))
        {
            throw new NotImplementedException("If command is not implemented yet");
        }

        throw new System.Exception("nie udalo sie wygenerowac komendy");
    }

    public int GetJumpLineNumber(List<CodeLine> solution, GameObject go)
    {
        return GetIndexOnTheList(solution, go.GetComponent<JumpInstructionScript>().bindedInstruction);
    }     

    public List<ICommand> GetCommands(List<CodeLine> solution)
    {
        List<ICommand> commandList = new List<ICommand>();

        for (int i = 0; i < solution.Count; i++)
        {
            var command = GetCommand(solution, i);
            commandList.Add(command);
        }

        return commandList;
    }

    public int GetIndexOnTheList(List<CodeLine> solution, GameObject go)
    {
        for(int i=0; i<solution.Count; i++)
        {
            if (go == solution[i].go) return i;
        }

        return -1;
    }
}

public class InstructionHelper
{
    public static bool IsJumpInstruction(GameObject go)
    {
        return go.GetComponent<JumpInstructionScript>() != null;
    }

    public static bool IsJumpInstructionLabel(GameObject go)
    {
        return go.GetComponent<JumpInstructionScript>().IsLabel;
    }

    public static bool IsPutInstruction(GameObject go)
    {
        return go.GetComponent<PutInstructionScript>() != null;
    }

    public static bool IsPickInstruction(GameObject go)
    {
        return go.GetComponent<PickInstructionScript>() != null;
    }

    public static bool IsMoveInstruction(GameObject go)
    {
        return go.GetComponent<MoveInstructionScript>() != null;
    }

    public static bool IsIfInstruction(GameObject go)
    {
        return go.GetComponent<IfElseInstructionScript>() != null;
    }

    public static bool IsAddInstruction(GameObject go)
    {
        return go.GetComponent<AddInstructionScript>() != null;
    }

    public static bool IsSubInstruction(GameObject go)
    {
        return go.GetComponent<SubInstructionScript>() != null;
    }

    public static bool IsDecInstruction(GameObject go)
    {
        return go.GetComponent<DecInstructionScript>() != null;
    }

    public static bool IsIncInstruction(GameObject go)
    {
        return go.GetComponent<IncInstructionScript>() != null;
    }

    public static Direction GetInstructionDirection(GameObject go)
    {
        Direction direction = go.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>().SelectedDirection;
        return direction;
    }
}


public class CodeLine
{
    public GameObject go;
    public const float SizeY = 40; //height of the single line
    public const float SpacingY = 20; // height of the space between two lines

    public float lockCageTopY;
    public float lockCageBotY;

    public float LockCageSize => SizeY + SpacingY;

    public Vector2 dockPosition { get; set; }

    private Vector2 velocity;

    private Vector2 shift;

    public Vector2 BlockSize { get
        {
            var width = 150;
            var height = SizeY + SpacingY;
            return new Vector2(width, height);
        } }

    public Vector2 GetDockPositionWithScroll(Vector2 scrollVector)
    {
        return dockPosition - scrollVector;
    }

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
        newDockPosition = GetPositionTopDown(newDockPosition.x, newDockPosition.y);
        //RepairShift(dockPosition, newDockPosition);
        this.dockPosition = newDockPosition;
        lockCageTopY = dockPosition.y + LockCageSize;
        lockCageBotY = dockPosition.y - LockCageSize;
    }

    public void RepairShift(Vector2 oldPos, Vector2 newPos)
    {
        Vector2 midpoint = oldPos + shift;
        shift = newPos - midpoint;
    }

    public void SetTopBot(bool isTop, bool isBottom)
    {
        lockCageTopY = dockPosition.y + LockCageSize;
        lockCageBotY = dockPosition.y - LockCageSize;
    }

    public Vector2 GetPositionTopDown(float x, float y)
    {
        var parentRectTransform = go.transform.parent.GetComponent<RectTransform>();
        var parentWidth = parentRectTransform.rect.width;
        var parentHeight = parentRectTransform.rect.height;

        var thisRectTransform = go.transform.GetComponent<RectTransform>();
        var thisWidth = thisRectTransform.rect.width;
        var thisHeight = thisRectTransform.rect.height;

        Vector2 newPosition = new Vector2(x - parentWidth / 2 + thisWidth / 2, y - parentHeight / 2 - thisHeight / 2);
        return newPosition;
    }

    //public void SetPositionTopDown(float x, float y)
    //{
    //    var newPosition = GetPositionTopDown(x, y);
    //    go.GetComponent<RectTransform>().anchoredPosition = newPosition;
    //}

    public const float MinDistanceForRepelForce = 10f;
    public const float MaxDistanceForRepelForce = SizeY + SpacingY; // height of the single element
    public const float ReturnForceScaleFactor = 0.08f;
    public const float RepelForceScaleFactor = 8;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="forceSourcePosition">Mouse position AFTER applying the scroll</param>
    public Vector2 GetRepelForce(Vector2 forceSourcePosition, Vector2 scrollVector)
    {
        Vector2 relativeDock = GetDockPositionWithScroll(scrollVector);

        float yDiff = relativeDock.y - forceSourcePosition.y + scrollVector.y;

        yDiff = Mathf.Clamp(yDiff, -MinDistanceForRepelForce, MinDistanceForRepelForce);

        if (yDiff >= 0 && yDiff < MinDistanceForRepelForce) yDiff = MinDistanceForRepelForce;
        if (yDiff <= 0 && yDiff > -MinDistanceForRepelForce) yDiff = -MinDistanceForRepelForce;

        return new Vector2(0, RepelForceScaleFactor / Mathf.Pow(Mathf.Abs(yDiff), 0.6f) * Mathf.Sign(yDiff));
    }

    public Vector2 GetReturnVector(Vector2 scrollVector)
    {
        //float yDiff = GetDockPositionWithScroll(scrollVector).y - go.transform.position.y;

        //if (yDiff == 0) return Vector2.zero;
        //if (yDiff >= 0 && yDiff < 0.1) yDiff = 0.1f;
        //if (yDiff <= 0 && yDiff > -0.1) yDiff = -0.1f;

        //Vector2 newForce = new Vector2(0, ReturnForceScaleFactor * yDiff);
        //return newForce;

        return -shift / 10;
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

    public void UpdatePosition(bool isAnythingDragged, bool isThisOneDragged, float scrollY, Vector2 relMousePosition)
    {
        if (isThisOneDragged)
        {
            DrawContour(relMousePosition);
            return;
        }

        Vector2 sumForce = Vector2.zero;

        Vector2 scrollVector = new Vector2(0, scrollY);
        Vector2 relativeMousePosition = relMousePosition;// (Vector2)Input.mousePosition - scrollVector;

        if (isAnythingDragged)
        {
            sumForce = GetRepelForce(relativeMousePosition, scrollVector) + GetReturnVector(scrollVector);
        }
        else
        {
            sumForce = GetReturnVector(scrollVector);
        }

        velocity = sumForce;

        shift += velocity;

        //Vector2 newPosition = new Vector2(go.transform.position.x + velocity.x, go.transform.position.y + velocity.y);
        //Vector2 newPosition = new Vector2(dockPosition.x, scrollY + go.transform.position.y + velocity.y);
        Vector2 newPosition = new Vector2(dockPosition.x, shift.y + GetDockPositionWithScroll(scrollVector).y);

        //go.transform.position = newPosition;
        go.GetComponent<RectTransform>().anchoredPosition = newPosition;
        //CheckIfOutOfBounds();

        DrawContour();

        if (InstructionHelper.IsJumpInstruction(this.go))
        {
            if (this.go.GetComponent<JumpInstructionScript>().arrow == null) Debug.Log("Null at arrow");
            if (this.go.GetComponent<JumpInstructionScript>().arrow.GetComponent<JumpInstructionArrow>() == null) Debug.Log("Null at arrow script");
            this.go.GetComponent<JumpInstructionScript>().arrow.GetComponent<JumpInstructionArrow>().UpdateCurve();
        }
    }

    string guid = null;
    bool drawContour = false;

    public void DrawContour(Vector2? position = null)
    {
        if (!drawContour) return;

        GameObject instance = null;      

        if (guid == null)
        {
            System.Random rng = new System.Random();
            guid = Guid.NewGuid().ToString();
            var debugRectPrefab = GameObject.Find("DebugRect");
            instance = GameObject.Instantiate(debugRectPrefab);
            instance.name = "DebugRect" + guid;
            instance.transform.SetParent(go.transform.parent);

            instance.GetComponent<Image>().color = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
        }
        else
        {
            instance = GameObject.Find("DebugRect" + guid).gameObject;
        }

        var debugRectTransform = instance.GetComponent<RectTransform>();
        debugRectTransform.anchoredPosition = position ?? dockPosition;
        debugRectTransform.sizeDelta = go.GetComponent<RectTransform>().sizeDelta * 1.2f;
        //Rect r = go.GetComponent<RectTransform>().rect;
                
        //Vector3 topleft = new Vector3(r.xMin, r.yMax, 150);
        //Vector3 topright = new Vector3(r.xMax, r.yMax, 150);
        //Vector3 downleft = new Vector3(r.xMin, r.yMin, 150);
        //Vector3 downright = new Vector3(r.xMax, r.yMin, 150);

        //Debug.DrawLine(topleft, topright, Color.red, 1f, false);
        //Debug.DrawLine(topright, downright, Color.red, 1f, false);
        //Debug.DrawLine(downright, downleft, Color.red, 1f, false);
        //Debug.DrawLine(downleft, topleft, Color.red, 1f, false);
    }

}
