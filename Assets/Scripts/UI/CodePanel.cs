using Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private const int numberOfSolutions = 1;
    private static int currentSolutionIndex = 0;

    public List<CodeLine> CurrentSolution => solutions[currentSolutionIndex];

    private float scrollY;

    private float CanvasScale => GameObject.Find("LevelCanvas").GetComponent<Canvas>().scaleFactor;

    private static Vector2 topLeftCorner;
    private static Rect panelRect;

    private void InitializeSolutions()
    {
        solutions = new List<List<CodeLine>>();
        for (int i = 0; i < numberOfSolutions; i++)
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
        InitializeGhostInstruction();

        draggedObject = null;
        scrollY = 0;
    }

    public void InitializeGhostInstruction()
    {
        ghostInstructionBlock = GameObject.Find("GhostInstruction");
        ghostInstructionBlock.transform.SetParent(GameObject.Find("NotVisible").transform);
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

    void FixedUpdate()
    {
        if (draggedObject != null && IsMouseInCodePanel())
        {
            HandleDrag();
        }
        else
        {
            RemoveGhostInstruction();
        }

        UpdateScroll();
        SetPositions();
    }



    int LastGhostBlockIndex { get; set; } = -1;

    GameObject ghostInstructionBlock;

    public void HandleDrag()
    {
        ShowGhostInstruction();
    }

    public Vector2 CalculatePositionForGhostInstruction(Vector2 absoluteDockPosition)
    {
        float x = absoluteDockPosition.x;
        float y = absoluteDockPosition.y;
        var parentRectTransform = GetComponent<RectTransform>();
        var parentWidth = parentRectTransform.rect.width;
        var parentHeight = parentRectTransform.rect.height;

        var thisRectTransform = GameObject.Find("GhostInstruction").GetComponent<RectTransform>();
        var thisWidth = thisRectTransform.rect.width;
        var thisHeight = thisRectTransform.rect.height;

        Vector2 newPosition = new Vector2(x - parentWidth / 2 + thisWidth / 2, y - parentHeight / 2 - thisHeight / 2);
        newPosition -= new Vector2(0, scrollY);
        return newPosition;
    }

    public Vector2 GetOffsetForGhostInstruction(CodeLine ifLine)
    {
        if (ifLine == null) return Vector2.zero;
        return new Vector2((ifLine.Indent + 1) * CodeLine.IndentPixelMultiplifier, 0);
    }

    public void ShowGhostInstruction()
    {
        int ghostBlockIndex = GetSlotIndexUnderMousePosition();
        CodeLine ifLine = GetIfBlockUnderMousePosition();
        //if (LastGhostBlockIndex == ghostBlockIndex) return;

        var dockPosition = CalculatePositionForGhostInstruction(GetAbsoluteDockPositionForIndex(ghostBlockIndex)) + GetOffsetForGhostInstruction(ifLine);

        ghostInstructionBlock.transform.SetParent(transform);
        ghostInstructionBlock.GetComponent<RectTransform>().anchoredPosition = dockPosition; //plus scroll!

        LastGhostBlockIndex = ghostBlockIndex;
    }

    public void RemoveGhostInstruction()
    {
        LastGhostBlockIndex = -1;
        ghostInstructionBlock.transform.SetParent(GameObject.Find("NotVisible").transform);
    }

   

    private bool IsMouseInCodePanel()
    {
        var absoluteRect = gameObject.GetComponent<RectTransform>().rect;
        Rect relativeRect = new Rect(-absoluteRect.width / 2, absoluteRect.height / 2, absoluteRect.width, absoluteRect.height);
        Vector2 position = GetMousePositionOnCodePanel();
        bool isIn = IsInRect(relativeRect, position);
        return isIn;
    }

    public void SetPositions()
    {
        bool isAnythingDragged = draggedObject != null && IsMouseInCodePanel();

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
        if (eventData.pointerDrag.gameObject.name == "InstructionBank") return;
        if (eventData.pointerDrag != null)
        {
            HandleDrop(eventData);
        }
    }

    private void HandleDrop(PointerEventData eventData)
    {
        RemoveGhostInstruction();

        HandleMoveInstructionFirstDrop(eventData);
        HandleIfInstructionFirstDrop(eventData);
        ShowDirectionIndicatorIfNeeded(eventData);
        ShowComparisonTypeIndicatorIfNeeded(eventData);
        ShowDropDownIfNeeded(eventData);

        RaycastManagerScript.SetRaycastBlockingAfterInstructionReleased();

        int index = GetSlotIndexUnderMousePosition();

        CodeLine parent = GetIfBlockUnderMousePosition();

        InsertAtSlot(index, eventData.pointerDrag, parent);
    }   

    private CodeLine GetIfBlockUnderMousePosition()
    {
        if (draggedObject == null) return null; 

        var mousePosition = GetMousePositionOnCodePanel();
        for (int i = CurrentSolution.Count - 1; i >= 0; i--)
        {
            if (!InstructionHelper.IsIfInstruction(CurrentSolution[i].go))
            {
                continue;
            }

            Rect nest = CurrentSolution[i].Nest;
            Rect nestScrolled = new Rect(nest.position + new Vector2(0, scrollY), nest.size);

            if (IsInRect(nestScrolled, mousePosition))
            {
                CurrentSolution[i].TemporaryCodeLineNestHeight = unpinnedCodeline?.NestHeight ?? CodeLine.DefaultHeight;
                //SetDebugNestInRect(nest);
                return CurrentSolution[i];
            }
            else
            {
                CurrentSolution[i].TemporaryCodeLineNestHeight = 0;
            }
        }
        return null;
    }

    private void SetDebugNestInRect(Rect rect)
    {
        var debugRect = GameObject.Find("DebugRect");
        var debugRectTransform = debugRect.GetComponent<RectTransform>();
        debugRectTransform.anchoredPosition = rect.center + new Vector2(0, -rect.height); //y flip
        debugRectTransform.sizeDelta = rect.size * 1.0f;
        debugRect.transform.SetParent(transform.root);
        debugRect.transform.SetParent(GameObject.Find("SolutionPanel").transform);
    }

    private void HandleMoveInstructionFirstDrop(PointerEventData eventData)
    {
        if (!InstructionHelper.IsMoveInstruction(eventData.pointerDrag)) return;
        if (unpinnedCodeline != null) return;

        ShowDirectionIndicatorIfNeeded(eventData);
        StartCoroutine(CoroutineExpandDirectionIndicator(eventData, eventData.pointerDrag.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>()));
    }

    private void ShowDirectionIndicatorIfNeeded(PointerEventData eventData)
    {
        if (eventData.pointerDrag.transform.Find("DirectionIndicator") != null)
        {
            eventData.pointerDrag.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>().Show();
        }
    }

    private void HandleIfInstructionFirstDrop(PointerEventData eventData)
    {
        if (!InstructionHelper.IsIfInstruction(eventData.pointerDrag)) return;
        if (unpinnedCodeline != null) return;

        ShowDirectionIndicatorIfNeeded(eventData);
        ShowComparisonTypeIndicatorIfNeeded(eventData);
        ShowDropDownIfNeeded(eventData);
        //StartCoroutine(CoroutineExpandIfBlockOnFirstDrop(eventData.pointerDrag.GetComponent<RectTransform>()));
        StartCoroutine(CoroutineHandleIfInstructionDrop(eventData, 
            eventData.pointerDrag.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>(),
            eventData.pointerDrag.transform.Find("ComparisonIndicator").GetComponent<ComparisonTypeIndicatorScript>()
            ));        
    }

    private IEnumerator CoroutineExpandIfBlockOnFirstDrop(RectTransform rectTransform)
    {
        yield return new WaitForFixedUpdate();
        var currentWidth = rectTransform.sizeDelta.x;
        var desiredWidth = currentWidth + 50;

        for (int i=0; i<30; i++)
        {
            var oldWidth = currentWidth;
            currentWidth = Mathf.Lerp(currentWidth, desiredWidth, 0.2f);
            var newSize = new Vector2(currentWidth, rectTransform.sizeDelta.y);
            var newPosition = rectTransform.anchoredPosition + new Vector2((currentWidth - oldWidth), 0);
            rectTransform.anchoredPosition = newPosition;
            rectTransform.sizeDelta = newSize;
        }
    }

    private IEnumerator CoroutineHandleIfInstructionDrop(PointerEventData eventData, DirectionIndicatorScript directionScript, ComparisonTypeIndicatorScript comparisonScript)
    {
        yield return new WaitForFixedUpdate();
        yield return CoroutineExpandDirectionIndicator(eventData, directionScript);
        yield return new WaitUntil(() => directionScript.SelectedDirection != null);
        yield return new WaitForSeconds(0.2f);
        ExpandComparisonTypeIndicator(eventData, comparisonScript);
    }

    private IEnumerator CoroutineExpandDirectionIndicator(PointerEventData eventData, DirectionIndicatorScript directionScript)
    {
        yield return new WaitForFixedUpdate();
        directionScript.OnPointerClick(eventData);
    }

    private void ExpandComparisonTypeIndicator(PointerEventData eventData, ComparisonTypeIndicatorScript comparisonScript)
    {
        comparisonScript.OnPointerClick(eventData);
    }

    private void ShowComparisonTypeIndicatorIfNeeded(PointerEventData eventData)
    {
        if (eventData.pointerDrag.transform.Find("ComparisonIndicator") != null)
        {
            eventData.pointerDrag.transform.Find("ComparisonIndicator").GetComponent<ComparisonTypeIndicatorScript>().Show();
        }
    }

    private void ShowDropDownIfNeeded(PointerEventData eventData)
    {
        if (eventData.pointerDrag.transform.Find("Dropdown") != null)
        {
            eventData.pointerDrag.transform.Find("Dropdown").gameObject.SetActive(true);
        }
    }

    private Vector2 TranslateMousePositionToPanel(Vector2 position)
    {
        var middle = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        var transformed = position - middle;
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

    private int GetSlotIndexUnderMousePosition()
    {
        var mousePositionOnPanel = GetMousePositionOnCodePanel();

        if (CurrentSolution.Count == 0) return 0;

        else
        {
            if (mousePositionOnPanel.y >= CurrentSolution[0].dockPosition.y) // add at the top
            {
                return 0;
            }

            else if (mousePositionOnPanel.y < CurrentSolution[CurrentSolution.Count - 1].dockPosition.y)//add at the bottom
            {
                if (draggedObject == CurrentSolution[CurrentSolution.Count - 1].go) return CurrentSolution.Count - 1;
                return CurrentSolution.Count;
            }

            else
            {
                for (int i = 0; i < CurrentSolution.Count - 1; i++)
                {
                    if (mousePositionOnPanel.y <= CurrentSolution[i].dockPosition.y && mousePositionOnPanel.y > CurrentSolution[i + 1].dockPosition.y)
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

    private void InsertAtSlot(int index, GameObject go, CodeLine parent)
    {
        if (unpinnedCodeline != null)
        {
            Pin(unpinnedCodeline, ref index);
            UnzipHierarchy(unpinnedCodeline);
            unpinnedCodeline.SetParent(parent);
            unpinnedCodeline = null;
            Rearrange();
            return;
        }

        int indexOfPresentLine = GetGameObjectIndexOnList(go); //check if the dragged object is already on the code panel

        if (indexOfPresentLine >= 0)
        {
            CodeLine draggedLine = CurrentSolution[indexOfPresentLine];
            if (parent != draggedLine && draggedLine.HasChildInHierarchy(parent))
            {
                Debug.LogWarning("User tried to do impossible nesting.");
                return;
            }

            draggedLine.SetParent(parent);
            CurrentSolution.RemoveAt(indexOfPresentLine);

            if (index > indexOfPresentLine) index--;
            CurrentSolution.Insert(index, draggedLine);
        }

        else
        {
            InsertNormalInstruction(index, go, parent);
        }

        if (InstructionHelper.IsJumpInstruction(go) && indexOfPresentLine < 0)
        {
            InsertJumpInstruction(index, go, parent);
        }

        Rearrange();
    }

    public void InsertNormalInstruction(int index, GameObject go, CodeLine parent)
    {
        Vector2 newDockPosition = GetAbsoluteDockPositionForIndex(index);
        CodeLine newCodeLine = new CodeLine(go, newDockPosition, parent);
        CurrentSolution.Insert(index, newCodeLine);
        go.transform.SetParent(transform);
    }

    public void InsertJumpInstruction(int index, GameObject go, CodeLine parent)
    {
        GameObject jumpInstructionLabel = GetJumpInstructionLabel(go);
        Vector2 newDockPosition = GetAbsoluteDockPositionForIndex(index);
        CodeLine newCodeLine = new CodeLine(jumpInstructionLabel, newDockPosition, parent);
        CurrentSolution.Insert(index, newCodeLine);
        go.GetComponent<JumpInstructionScript>().AttachArrow();
    }

    public void Remove(GameObject go, CodeLine codeLine = null)
    {        
        RemoveGhostInstruction();

        var lineToRemove = unpinnedCodeline;

        if (lineToRemove != null)
        {
            if (InstructionHelper.IsIfInstruction(go))
            {
                CodeLine ifInstruction = codeLine ?? unpinnedCodeline;
                for (int i = ifInstruction.children.Count - 1; i >= 0; i--)
                {
                    var child = ifInstruction.children[i];
                    Remove(child.go, child);
                    child.SetParent(null);
                }
            }

            if (InstructionHelper.IsJumpInstruction(go))
            {
                GameObject bindedInstruction = go.GetComponent<JumpInstructionScript>().bindedInstruction;
                if (bindedInstruction != null)
                {
                    go.GetComponent<JumpInstructionScript>().bindedInstruction = null;                    
                    CurrentSolution.RemoveAt(GetGameObjectIndexOnList(bindedInstruction));
                    Destroy(bindedInstruction);
                }
            }

            CurrentSolution.Remove(lineToRemove);
            foreach (var line in CurrentSolution)
            {
                if (line.children.Contains(lineToRemove))
                {
                    line.children.Remove(lineToRemove);
                    break;
                }
        }
            Destroy(go);
        }  
        
        else
        {
            Destroy(go);
        }

        

        unpinnedCodeline = null;

        Rearrange();
    }

    public float BlockSizeY = 60;

    public float GetYPositionForFirstChild()
    {
        return -BlockSizeY;
    }

    public float GetIndentX(Vector2 parentSize, Vector2 childSize)
    {
        return -parentSize.x / 2 + CodeLine.IndentPixelMultiplifier + childSize.x / 2;
    }

    //public Vector2 GetDockPositionForChild(Vector2 parentSize, Vector2 childSize, int index)
    //{
    //    return GetDockPositionForFirstChild(parentSize, childSize) + new Vector2(0, index * BlockSizeY);
    //}

    public void ZipHierarchy(CodeLine parent)
    {
        var parentRT = parent.go.GetComponent<RectTransform>();
        var currentPositionY = GetYPositionForFirstChild();
        foreach(var child in parent.children)
        {
            child.go.transform.SetParent(parent.go.transform);
            var childRT = child.go.GetComponent<RectTransform>();
            childRT.anchoredPosition = new Vector2(GetIndentX(parentRT.sizeDelta, childRT.sizeDelta), currentPositionY);
            currentPositionY -= child.NestSize.y;
            ZipHierarchy(child);
        }
    }

    public void UnzipHierarchy(CodeLine parent)
    {
        parent.go.transform.SetParent(transform);
        foreach(var child in parent.children)
        {
            UnzipHierarchy(child);
        }
    }

    public void Unpin(CodeLine parent)
    {
        CurrentSolution.Remove(parent);
        foreach(var child in parent.children)
        {
            Unpin(child);
        }
    }

    public CodeLine unpinnedCodeline = null;

    //returns index of the next location
    public void Pin(CodeLine parent, ref int index)
    {
        CurrentSolution.Insert(index, parent);
        foreach (var child in parent.children)
        {
            index++;
            Pin(child, ref index);
        }
    }

    private void RearrangeChildren(int parentIndex)
    {
        var parentCodeLine = CurrentSolution[parentIndex];

        for (int i = parentCodeLine.children.Count - 1; i >= 0; i--)
        {
            var child = parentCodeLine.children[i];
            var childIndex = GetGameObjectIndexOnList(parentCodeLine.children[i].go);
            if (InstructionHelper.IsIfInstruction(child.go))
            {
                RearrangeChildren(childIndex);
            }
            CurrentSolution.Remove(child);
            CurrentSolution.Insert(parentIndex + 1, child);
        }
    }

    public void Rearrange()
    {
        for (int i = 0; i < CurrentSolution.Count; i++)
        {
            CurrentSolution[i].ChangeDockPosition(GetAbsoluteDockPositionForIndex(i));
            CurrentSolution[i].RevalidateChildren();
            CurrentSolution[i].TemporaryCodeLineNestHeight = 0;
        }
    }

    private Vector2 GetAbsoluteDockPositionForIndex(int index)
    {
        var calculatedDockPosition = GetAbsoluteDockPositionForFirstBlock();
        //List<int> extraSpacingOn = new List<int>();

        for (int i = 0; i < index; i++)
        {
            var current = CurrentSolution[i];
            var nextBlockSize = current.BlockSpacing.y;
            int childrenCount = current.GetAllChildrenCount();
            if (InstructionHelper.IsIfInstruction(current.go))
            {
                //Debug.Log($"children: {childrenCount}");
                //extraSpacingOn.Add(i + childrenCount);
            }

           // int extraIndents = extraSpacingOn.RemoveAll(x => x == i);
            //nextBlockSize += extraIndents * 10;

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
        scrollVelocity += eventData.scrollDelta.y * ScrollMultiplier;        
    }

    private const float scrollFrameDecay = 0.15f;
    private float scrollVelocity = 0.0f;
    private float maxScrollVelocity = 8.0f;

    public void UpdateScroll()
    {
        var scrollVelocitySign = Mathf.Sign(scrollVelocity);
        scrollVelocity = scrollVelocitySign * Mathf.Max(0, Mathf.Abs(scrollVelocity) - scrollFrameDecay);
        scrollY += scrollVelocity;
        TrimScroll();
        TrimScrollVelocity();
    }

    private void TrimScroll()
    {
        scrollY = Mathf.Min(0, scrollY);
    }

    private void TrimScrollVelocity()
    {
        scrollVelocity = Mathf.Clamp(scrollVelocity, -maxScrollVelocity, maxScrollVelocity);
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
            int trueLineNumber = currentLine + 1;
            int elseLineNumber = currentLine + line.children.Count + 1;
            return new IfCommand(trueLineNumber, elseLineNumber, GetConditions(line), GetLogicalOperators(line));
        }

        throw new System.Exception("Could not generate a command.");
    }

    public List<Condition> GetConditions(CodeLine ifLine)
    {
        var directionIndicator = ifLine.go.transform.Find("DirectionIndicator");
        var comparisonIndicator = ifLine.go.transform.Find("ComparisonIndicator");
        var relationIndicator = ifLine.go.transform.Find("Dropdown").Find("Label");
        var relationIndicatorText = relationIndicator.GetComponent<Text>().text;

        Condition condition = new Condition()
        {
            leftSide = directionIndicator.GetComponent<DirectionIndicatorScript>().SelectedDirection.Value,
            comparisonSign = ParseComparisonSign(relationIndicatorText),
            rightSide = GetComparisonObject(comparisonIndicator.gameObject)
        };

        return new List<Condition>() { condition };
    }

    public ComparisonObject GetComparisonObject(GameObject comparisonIndicator)
    {
        var script = comparisonIndicator.GetComponent<ComparisonTypeIndicatorScript>();
        ComparisonObject comparisonObject = new ComparisonObject();
        comparisonObject.comparedType = script.SelectedComparisonType;
        return comparisonObject;
    }

    public ComparisonSign ParseComparisonSign(string value)
    {
        switch (value)
        {
            case "==": return ComparisonSign.Equal;
            case "!=": return ComparisonSign.NotEqual;
            case "<=": return ComparisonSign.LesserEqual;
            case ">=": return ComparisonSign.GreaterEqual;
            case ">": return ComparisonSign.Greater;
            case "<": return ComparisonSign.Lesser;
            default: throw new Exception("Invalid string in comparison sign parsing.");
        }
    }

    public List<LogicalOperator> GetLogicalOperators(CodeLine ifLine)
    {
        return new List<LogicalOperator>() { };
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

        commandList.Add(new FinishCommand());
        return commandList;
    }

    public int GetIndexOnTheList(List<CodeLine> solution, GameObject go)
    {
        for (int i = 0; i < solution.Count; i++)
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
        Direction direction = go.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>().SelectedDirection.Value;
        return direction;
    }
}


public class CodeLine
{
    public GameObject go;

    public List<CodeLine> children = new List<CodeLine>();
    public CodeLine parent = null; //null means not being in an if block

    public const float SizeY = 40; //height of the single line
    public const float SpacingY = 20; // height of the space between two lines

    public float LockCageSize => SizeY + SpacingY;

    public Vector2 dockPosition { get; set; }

    private Vector2 velocity;

    private Vector2 shift;

    private GameObject nestBackground;

    public bool HasTemporaryCodeLine => TemporaryCodeLineNestHeight > 0;

    public float TemporaryCodeLineNestHeight { get; set; }
    public static float DefaultBackgroundSizeChange => DefaultHeight;
    private float _desiredBackgroundHeight;

    public const float MinimalBackgroundNestHeight = 0;
    private float DesiredBackgroundHeight
    {
        get
        {
            return NestHeight - (SpacingY + SizeY) + TemporaryCodeLineNestHeight;
        }
    }

    public Vector2 BlockSpacing
    {
        get
        {
            var width = 150;
            var height = SizeY + SpacingY;
            //if (IsLastChild() && !InstructionHelper.IsIfInstruction(go)) height += MinimalBackgroundNestHeight;
            //if (children.Count == 0 && InstructionHelper.IsIfInstruction(go)) height += MinimalBackgroundNestHeight;
            return new Vector2(width, height);
        }
    }

    public Vector2 BlockSize
    {
        get
        {
            var rect = go.GetComponent<RectTransform>().rect;
            return new Vector2(rect.width, rect.height);
        }
    }

    public Vector2 TopLeft
    {
        get
        {
            Vector2 size = BlockSize;
            Vector2 topLeft = dockPosition + new Vector2(-size.x / 2, size.y / 2);
            return topLeft;
        }
    }

    public Vector2 DownLeft
    {
        get
        {
            Vector2 size = BlockSize;
            Vector2 downLeft = dockPosition + new Vector2(-size.x / 2, -size.y / 2);
            return downLeft;
        }
    }

    public Rect Inside
    {
        get
        {
            Vector2 size = BlockSize;
            Vector2 downLeft = dockPosition - new Vector2(size.x / 2, -size.y / 2);
            return new Rect(downLeft, size);
        }
    }

    public const float DefaultHeight = SizeY + SpacingY;

    public float ChildrenHeight
    {
        get
        {
            float sumHeight = 0;
            foreach (var child in children)
            {
                sumHeight += child.NestHeight;
            }

            return sumHeight;
        }
    }

    public const int IndentPixelMultiplifier = 40;

    public int Indent
    {
        get
        {
            if (parent == null) return 0;
            return parent.Indent + 1;
        }
    }

    public int IndentInPixels => Indent * IndentPixelMultiplifier;

    public float NestHeight => DefaultHeight + ChildrenHeight + IfBonusForNestHeight;

    public float NestWidth => 300;

    public Vector2 NestSize => new Vector2(NestWidth, NestHeight);

    public Vector2 TopLeftWithRepel
    {
        get
        {
            Vector2 middle = go.GetComponent<RectTransform>().anchoredPosition;
            Vector2 size = BlockSize;
            Vector2 topLeft = middle + new Vector2(-size.x / 2, size.y / 2);
            return topLeft;
        }
    }
    public Rect Nest => new Rect(TopLeftWithRepel, NestSize);

    private int ifChildrenCount = 0;
    public float IfBonusForNestHeight => MinimalBackgroundNestHeight + MinimalBackgroundNestHeight * ifChildrenCount;

    public Vector2 GetDockPositionWithScroll(Vector2 scrollVector)
    {
        return dockPosition - scrollVector;
    }

    public CodeLine(GameObject go, Vector2 dockPosition, CodeLine parent)
    {
        this.velocity = Vector2.zero;

        this.go = go;
        this.dockPosition = dockPosition;
        go.transform.position = dockPosition;

        this.SetParent(parent);

        if (InstructionHelper.IsIfInstruction(go))
        {
            CreateNestBackgroundIfNeeded();
        }
    }

    public bool HasChildInHierarchy(CodeLine child)
    {
        if (child == this) return true;

        foreach (var v in children)
        {
            if (v.HasChildInHierarchy(child)) return true;
        }

        return false;
    }

    public void SetParent(CodeLine newParent)
    {
        if (newParent == this) return;

        if (parent != null)
        {
            parent.RemoveChild(this);
        }

        parent = newParent;

        if (newParent != null)
        {
            newParent.AddChild(this);
        }
    }

    public bool IsLastChild()
    {
        if (parent == null) return false;
        return parent.children.Last() == this; 
    }

    private void CreateNestBackgroundIfNeeded()
    {
        if (nestBackground != null) return;
        //nestBackground = GameObject.Instantiate(GameObject.Find("NestBackground"));
        //nestBackground.transform.SetParent(go.transform);
        nestBackground = go.transform.Find("NestBackground").gameObject;
        _desiredBackgroundHeight = MinimalBackgroundNestHeight;
    }    

    public void ExpandBackground(float backgroundSizeChange)
    {
        CreateNestBackgroundIfNeeded();
        if (parent != null) parent.ExpandBackground(backgroundSizeChange);
        _desiredBackgroundHeight += backgroundSizeChange;
    }

    public void ShrinkBackground(float backgroundSizeChange)
    {
        CreateNestBackgroundIfNeeded();
        if (parent != null) parent.ExpandBackground(backgroundSizeChange);
        _desiredBackgroundHeight -= backgroundSizeChange;
        if (_desiredBackgroundHeight < MinimalBackgroundNestHeight)
        {
            _desiredBackgroundHeight = MinimalBackgroundNestHeight;
        }
    }

    public void RepairNestBackground()
    {
        _desiredBackgroundHeight = NestHeight;
    }

    public void AddChild(CodeLine child)
    {
        CreateNestBackgroundIfNeeded();
        ExpandBackground(child.NestHeight);
        children.Add(child);
        RefreshIfBonus();
    }

    public void RemoveChild(CodeLine child)
    {
        ShrinkBackground(child.NestHeight);
        children.Remove(child);
        RefreshIfBonus();
    }

    public int GetAllChildrenCount()
    {
        return children.Sum(x => x.GetAllChildrenCount()) + children.Count;
    }

    private void RefreshIfBonus()
    {
        ifChildrenCount = children.Where(x => InstructionHelper.IsIfInstruction(x.go)).Count();
    }

    public void ChangeDockPosition(Vector2 newDockPosition, int indent = 0)
    {
        newDockPosition = GetPositionTopDown(newDockPosition.x, newDockPosition.y);
        this.dockPosition = newDockPosition;
    }

    public void RepairShift(Vector2 oldPos, Vector2 newPos)
    {
        Vector2 midpoint = oldPos + shift;
        shift = newPos - midpoint;
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

        if (yDiff > 0) return new Vector2(0, 0); //we do not want to repel the upper blocks

        return new Vector2(0, RepelForceScaleFactor / Mathf.Pow(Mathf.Abs(yDiff), 0.45f) * Mathf.Sign(yDiff));
    }

    public Vector2 GetReturnVector(Vector2 scrollVector)
    {
        return -shift / 24;
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

        Vector2 newPosition = new Vector2(dockPosition.x + IndentInPixels, shift.y + GetDockPositionWithScroll(scrollVector).y);

        go.GetComponent<RectTransform>().anchoredPosition = newPosition;

        DrawContour();

        if (InstructionHelper.IsJumpInstruction(this.go))
        {
            if (this.go.GetComponent<JumpInstructionScript>().arrow == null) Debug.Log("Null at arrow");
            if (this.go.GetComponent<JumpInstructionScript>().arrow.GetComponent<JumpInstructionArrow>() == null) Debug.Log("Null at arrow script");
            this.go.GetComponent<JumpInstructionScript>().arrow.GetComponent<JumpInstructionArrow>().UpdateCurve();
        }

        UpdateBackgroundNestIfNeeded();
    }    

    private void UpdateBackgroundNestIfNeeded()
    {
        if (nestBackground == null) return;
        var rectTransform = nestBackground.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        var parentScale = nestBackground.transform.parent.GetComponent<RectTransform>().localScale;

        var newBackgroundHeight = Mathf.Lerp(rect.height, DesiredBackgroundHeight, 0.2f);

        var ifSize = BlockSize;

        var newPosition = new Vector2((NestWidth - ifSize.x) / 2, -ifSize.y / 2 + -newBackgroundHeight / 2 / parentScale.y);
        var newSize = Vector2.zero;// new Vector2(NestWidth, newBackgroundHeight);
        //Debug.Log($"parentScale: {parentScale}");

        rectTransform.anchoredPosition = newPosition;
        rectTransform.sizeDelta = newSize;
        rectTransform.localScale = Vector3.one / parentScale.x;


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
    }

    internal void RevalidateChildren()
    {
        children = children.Where(x => x.go != null).ToList();
        RefreshIfBonus();
    }
}
