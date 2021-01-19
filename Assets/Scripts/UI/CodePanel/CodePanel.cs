using Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CodePanel : MonoBehaviour, IDropHandler, IScrollHandler
    {
        private float ScrollMultiplier = 5;

        public static GameObject draggedObject;
        public CodeLine unpinnedCodeline;
        public CodeLine fakeSingleLine;
        public GameObject ghostInstruction;

        public List<CodeLine> Solution { get; private set; }

        public List<CodeLine> AllCodeLines
        {
            get
            {
                List<CodeLine> allLines = new List<CodeLine>();
                
                for(int i=0; i<Solution.Count; i++)
                {
                    allLines.Add(Solution[i]);
                    allLines.AddRange(Solution[i].AllChildrenInHierarchy);
                }

                return allLines;
            }
        }

        public void SetRaycastBlockingForAllInstructions(bool value)
        {
            SetRaycastBlockingRecursive(value, Solution);
        }

        private void SetRaycastBlockingRecursive(bool value, List<CodeLine> codeLines)
        {
            foreach(var line in codeLines)
            {
                line.instruction.GetComponent<CanvasGroup>().blocksRaycasts = value;
                line.container.GetComponent<CanvasGroup>().blocksRaycasts = value;
            }
        }

        private float scrollY;

        private void InitializeSolutions()
        {
            Solution = new List<CodeLine>();
        }

        // Start is called before the first frame update
        void Start()
        {
            InitializeSolutions();
            InitializeGhostInstruction();

            draggedObject = null;

            fakeSingleLine = CodeLineFactory.GetStandardCodeLine(GameObject.Instantiate(GameObject.Find("MoveInstruction")));
            fakeSingleLine.container.transform.SetParent(GameObject.Find("NotVisible").transform);
            //fakeSingleLine.instruction.transform.SetParent(GameObject.Find("NotVisible").transform);
            ghostInstruction = GameObject.Find("GhostInstruction");

            scrollY = 0;
        }

        public void InitializeGhostInstruction()
        {
            ghostInstruction = GameObject.Find("GhostInstruction");
            ghostInstruction.transform.SetParent(GameObject.Find("GhostContainer").transform);
        }

        public static bool IsInRect(Rect r, Vector2 pos)
        {
            return (pos.x >= r.x && pos.x <= r.x + r.width && pos.y <= r.y && pos.y >= r.y - r.height);
        }

        void FixedUpdate()
        {
            UpdateScroll();
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            var isMouseInCodePanel = IsMouseInCodePanel();
            var isAnythingDragged = draggedObject != null;
            var allCodeLines = AllCodeLines;

            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var allContainers = GetAllCodeLineContainersSorted();
                        
            for (int i=0; i<allCodeLines.Count; i++)
            {
                var currentLine = allCodeLines[i];
                currentLine.BaseUpdate();
            }

            if (isAnythingDragged)
            {
                CodeLine parentLine = GetParentBlockUnderMousePosition(allContainers);
                List<GameObject> childInstructions = GetChildListBasedOnRoot(parentLine);
                int index = GetIndexToInsertUnderMousePosition(childInstructions);

                var linesToCalculateRepel = GetCodeLinesToUpdatePhysics(parentLine);

                if (isMouseInCodePanel)
                {
                    for (int i = 0; i < linesToCalculateRepel.Count; i++)
                    {
                        var currentLine = linesToCalculateRepel[i];
                        if (IsThisOneDragged(currentLine)) continue;
                        currentLine.RepelUpdate(mousePosition);
                    }

                    var parentObject = parentLine?.container ?? RootContainer;
                    var siblings = parentLine != null ? parentLine.children : Solution;
                    Vector2 firstPosition = parentLine == null ? GetTopLeftForFirst(parentObject) : new Vector2(-150, -30);
                    ShowGhostInstruction(parentObject, siblings, index, firstPosition);
                }
                else
                {
                    ClearGhostInstruction();
                }
            }  
            else
            {
                ClearGhostInstruction();
            }

            Rearrange();
            UpdateFiller();
        }

        
        private void ShowGhostInstruction(GameObject parentObject, List<CodeLine> siblings, int index, Vector2 firstPosition)
        {            
            ghostInstruction.transform.parent.SetParent(parentObject.transform);
            var ghostContainerRT = ghostInstruction.transform.parent.gameObject.GetComponent<RectTransform>();

            if (siblings.Count == 0)
            {
                Vector2 topLeft = firstPosition;
                ghostContainerRT.anchoredPosition = topLeft + new Vector2(ghostContainerRT.sizeDelta.x / 2, -ghostContainerRT.sizeDelta.y / 2);
            }
            else
            {
                if (index > 0)
                {
                    ghostInstruction.transform.parent.SetParent(siblings[index - 1].container.transform.parent);
                    var childContainerRT = siblings[index - 1].container.GetComponent<RectTransform>();
                    var pos = childContainerRT.anchoredPosition;
                    var size = childContainerRT.sizeDelta;
                    var ghostSize = ghostContainerRT.sizeDelta;
                    ghostContainerRT.anchoredPosition = pos + new Vector2(0, -size.y / 2 - ghostSize.y / 2);
                }
                else
                {
                    ghostInstruction.transform.parent.SetParent(siblings[0].container.transform.parent);
                    var childContainerRT = siblings[0].container.GetComponent<RectTransform>();
                    var pos = childContainerRT.anchoredPosition;
                    var size = childContainerRT.sizeDelta;
                    var ghostSize = ghostContainerRT.sizeDelta;
                    ghostContainerRT.anchoredPosition = pos;
                }

                
            }
        }

        private void ClearGhostInstruction()
        {
            ghostInstruction.transform.parent.SetParent(GameObject.Find("NotVisible").transform);
        }

        private List<CodeLine> GetCodeLinesToUpdatePhysics(CodeLine codeLine)
        {
            if (draggedObject == null) return new List<CodeLine>();
            return GetCodeLineListBasedOnParent(codeLine);
        }

        public Vector3 CalculateOffsetForHeldContainer(GameObject container)
        {
            if (container.tag != "Container") return Vector2.zero;
            return unpinnedCodeline.instruction.transform.position - unpinnedCodeline.container.transform.position;
        }

        public void UpdateFiller()
        {
            var filler = GameObject.Find("FillerContainer");
            var root = RootContainer;
            //if (root.transform.childCount == 1) return;
            if (Solution.Count == 0) return;

            var lowestChild = Solution.Last().container.transform;// RootContainer.transform.GetChild(root.transform.childCount - 2);
            var childRT = lowestChild.GetComponent<RectTransform>();
            var bottomY = childRT.anchoredPosition.y - childRT.sizeDelta.y / 2;

            var fillerRT = filler.GetComponent<RectTransform>();
            var size = fillerRT.sizeDelta;            

            var newPosition = new Vector2(fillerRT.anchoredPosition.x, bottomY - size.y / 2);
            fillerRT.anchoredPosition = newPosition;
        }

        private bool IsThisOneDragged(CodeLine codeLine)
        {
            return codeLine == unpinnedCodeline;
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
        private bool IsMouseInCodePanel()
        {
            var absoluteRect = gameObject.GetComponent<RectTransform>().rect;
            Rect relativeRect = new Rect(-absoluteRect.width / 2, absoluteRect.height / 2, absoluteRect.width, absoluteRect.height);
            Vector2 position = GetMousePositionOnCodePanel();
            bool isIn = IsInRect(relativeRect, position);
            return isIn;
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
            StartCoroutine(CoroutineHandleIfInstructionDrop(eventData,
                eventData.pointerDrag.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>(),
                eventData.pointerDrag.transform.Find("ComparisonIndicator").GetComponent<ComparisonTypeIndicatorScript>()
                ));
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
            Vector2 afterScrollAdding = worldSpaceCoordinates;// + new Vector2(0, scrollY);
            Vector2 mousePositionOnCodePanel = TranslateMousePositionToPanel(afterScrollAdding);
            return mousePositionOnCodePanel;
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
            return commandHelper.GetCommands(AllCodeLines);
        }

        public void OnScroll(PointerEventData eventData)
        {
            ApplyScroll(eventData);
        }

        private void ApplyScroll(PointerEventData eventData)
        {
            scrollVelocity += -eventData.scrollDelta.y * ScrollMultiplier;
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
            var rootRT = RootContainer.GetComponent<RectTransform>();
            rootRT.anchoredPosition = new Vector2(0, scrollY);
        }

        private void TrimScroll()
        {
            var maxScroll = CalculateMaxScroll();
            var minScroll = 0;
            scrollY = Mathf.Clamp(scrollY, minScroll, maxScroll);
        }

        private float CalculateMaxScroll()
        {
            var fillerContainerRT = FillerContainer.GetComponent<RectTransform>();
            var codePanelRT = GetComponent<RectTransform>();

            var fillerTop = fillerContainerRT.anchoredPosition.y + fillerContainerRT.sizeDelta.y / 2;
            var codePanelHeight = codePanelRT.sizeDelta.y;

            var difference = Mathf.Abs(fillerTop) - codePanelHeight/2;
            return Mathf.Max(0, difference);
        }

        private void TrimScrollVelocity()
        {
            scrollVelocity = Mathf.Clamp(scrollVelocity, -maxScrollVelocity, maxScrollVelocity);
        }

        public List<GameObject> GetAllCodeLineContainers()
        {
            return GameObject.FindGameObjectsWithTag("Container").ToList();
        }

        public List<GameObject> GetAllCodeLineContainersSorted()
        {
            return GameObject.FindGameObjectsWithTag("Container").OrderByDescending(x => x.transform.position.y).ToList();
        }

        public CodeLine GetCorrespondingCodeLine(GameObject containerOrInstruction)
        {
            var fit = AllCodeLines.Where(x => x.container == containerOrInstruction || x.instruction == containerOrInstruction).ToList();
            if (fit.Any()) return fit.First();
            return null;
        }

        public GameObject FillerContainer => GameObject.Find("FillerContainer");

        public Rect GetContainerRect(GameObject container)
        {
            var realPosition = container.transform.position;
            var containerSize = container.GetComponent<RectTransform>().rect.size;
            Vector2 topLeft = new Vector2(realPosition.x - containerSize.x / 2, realPosition.y + containerSize.y / 2);
            return new Rect(topLeft, containerSize);
        }

        public int GetIndexToInsertUnderMousePosition(List<GameObject> childInstructions)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            childInstructions = childInstructions.OrderByDescending(x => x.transform.position.y).ToList();

            for (int i = childInstructions.Count-1; i>=0; i--)
            {
                var current = childInstructions[i];
                if (mousePosition.y < current.transform.position.y)
                {
                    return i+1;
                }
            }

            return 0;
        }

        private bool IsContainerUnpinned(GameObject container)
        {
            return container.transform.parent == UnpinnedCodeLineParentTransform;
        }   
        
        private void ClearTemporaryInstructionFlag(List<GameObject> allContainers)
        {
            for(int i=0; i<allContainers.Count; i++)
            {
                GameObject container = allContainers[i];
                var correspondingCodeLine = GetCorrespondingCodeLine(container);
                if (correspondingCodeLine == null) continue;
                correspondingCodeLine.TemporaryCodeLine = null;
            }
        }

        public CodeLine GetParentBlockUnderMousePosition(List<GameObject> allContainers)
        {
            ClearTemporaryInstructionFlag(allContainers);
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            for (int i = allContainers.Count - 1; i >= 0; i--)
            {
                GameObject container = allContainers[i];
                if (IsContainerUnpinned(container)) continue; //
                var correspondingCodeLine = GetCorrespondingCodeLine(container);
                if (correspondingCodeLine == null) continue;
                if (!InstructionHelper.IsGroupInstruction(correspondingCodeLine.instruction)) continue;

                if (correspondingCodeLine.IsInsideNest(mousePosition))
                {
                    if (draggedObject != null)
                    {
                        correspondingCodeLine.TemporaryCodeLine = unpinnedCodeline ?? fakeSingleLine;
                    }
                    return correspondingCodeLine;
                }
            }

            return null;
        }

        public static bool HasDraggedObjectAValidTag(PointerEventData eventData)
        {
            return HasGameObjectAValidTag(eventData.pointerDrag);
        }

        public static bool HasGameObjectAValidTag(GameObject go)
        {
            List<string> validTags = new List<string>() { "Instruction", "Container" };
            return (validTags.Contains(go.tag));
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!HasDraggedObjectAValidTag(eventData)) return;
            var allContainers = GetAllCodeLineContainersSorted();
            if (unpinnedCodeline != null)
            {
                allContainers.Remove(unpinnedCodeline.container);
            }

            CodeLine parentLine = GetParentBlockUnderMousePosition(allContainers);
            List<GameObject> childInstructions = GetChildListBasedOnRoot(parentLine);
            int index = GetIndexToInsertUnderMousePosition(childInstructions);

            if (parentLine != null) Debug.Log($"parentLine", parentLine.container);
            CodeLine lineToInsert = unpinnedCodeline ?? CodeLineFactory.GetStandardCodeLine(eventData.pointerDrag);

            InsertAtLine(lineToInsert, index, parentLine);

            HandlePostDrag(lineToInsert, eventData);
            InsertJumpLabelInstructionIfNeeded(lineToInsert, index, parentLine);

            Pin();           
        }

        private void HandlePostDrag(CodeLine lineToInsert, PointerEventData eventData)
        {
            HandleMoveInstructionFirstDrop(eventData);
            HandleIfInstructionFirstDrop(eventData);
            ShowDirectionIndicatorIfNeeded(eventData);
            ShowComparisonTypeIndicatorIfNeeded(eventData);
            ShowDropDownIfNeeded(eventData);

            RaycastManagerScript.SetRaycastBlockingAfterInstructionReleased();
        }

        private void InsertJumpLabelInstructionIfNeeded(CodeLine insertedLine, int index, CodeLine parentLine)
        {
            if (unpinnedCodeline != null) return;
            if (!InstructionHelper.IsJumpInstruction(insertedLine.instruction)) return;
            if (InstructionHelper.IsJumpInstructionLabel(insertedLine.instruction)) return;
            var jumpLabel = insertedLine.instruction.GetComponent<JumpInstructionScript>().CreateBindedLabel();
            var labelCodeLine = CodeLineFactory.GetStandardCodeLine(jumpLabel);
            InsertAtLine(labelCodeLine, index, parentLine);
            insertedLine.instruction.GetComponent<JumpInstructionScript>().AttachArrow();
        }

        public List<GameObject> GetChildListBasedOnRoot(CodeLine parentLine)
        {
            if (parentLine == null) return GetAllInstructionsWithoutParent();
            else return GetAllChildInstructions(parentLine);
        }

        public List<CodeLine> GetCodeLineListBasedOnParent(CodeLine parentLine)
        {
            if (parentLine == null) return Solution;
            else return parentLine.children;
        }

        public List<GameObject> GetAllChildInstructions(CodeLine parentLine)
        {
            List<GameObject> children = new List<GameObject>();
            foreach(var child in parentLine.children)
            {
                children.Add(child.instruction);
            }
            return children;
        }

        public List<GameObject> GetAllInstructionsWithoutParent()
        {
            List<GameObject> children = new List<GameObject>();
            var rootTransform = RootContainer.transform;
            Debug.Log($"rootcontainer child count: {RootContainer.transform.childCount}");
            for(int i=0; i<rootTransform.childCount; i++)
            {
                var currentChild = rootTransform.GetChild(i);
                if (!HasGameObjectAValidTag(currentChild.gameObject)) continue;
                var instruction = GetNthInstructionInRootComponent(i, rootTransform);
                children.Add(instruction);
            }
            return children;
        }

        public GameObject GetNthInstructionInRootComponent(int n, Transform root)
        {
            var nthContainer = root.GetChild(n);
            for(int i=0; i<nthContainer.childCount; i++)
            {
                var child = nthContainer.GetChild(i);
                if (child.tag == "Instruction") return child.gameObject;
            }

            throw new Exception("No instruction was present in a container.");
        }

        private GameObject RootContainer => GameObject.Find("RootContainer");

        public void InsertAtLine(CodeLine lineToInsert, int index, CodeLine parentLine)
        {
            GameObject parentGameObject;
            if (parentLine == null)
            {
                parentGameObject = RootContainer;
            }
            else
            {
                parentLine.TemporaryCodeLine = null;
                parentGameObject = parentLine.instruction;
            }

            lineToInsert.SetParent(parentLine, index);
            lineToInsert.container.transform.SetParent(parentGameObject.transform);

            int parentLineIndex = parentLine == null ? 0 : parentLine.InstructionIndex;

            if (parentLine == null)
            {
                Solution.Insert(index + parentLineIndex, lineToInsert);
            }

            lineToInsert.container.transform.SetSiblingIndex(index);
            lineToInsert.instruction.GetComponent<CanvasGroup>().blocksRaycasts = true;
            lineToInsert.container.GetComponent<CanvasGroup>().blocksRaycasts = true; 

            //Rearrange();
        }

        public Vector2 GetTopLeftForFirst(GameObject parentContainer)
        {
            var parent = parentContainer;
            var size = parent.GetComponent<RectTransform>().sizeDelta;
            return new Vector2(-size.x / 2, size.y / 2);
        }

        public void Rearrange()
        {            
            RearrangeIndices();
            RearrangePositions(Solution, GetTopLeftForFirst(RootContainer));
            UpdateFiller();
        }

        private void RemoveDeadJumpInstructions()
        {
            var instructions = AllCodeLines;
            for(int i=instructions.Count-1; i>=0; i--)
            {
                var current = instructions[i];
                if (InstructionHelper.IsJumpInstruction(current.instruction))
                {
                    var script = current.instruction.GetComponent<JumpInstructionScript>();
                    if (script.bindedInstruction == null)
                    {
                        RemoveCodeLineFromSolution(current);
                        DestroyCodeLine(current);
                    }
                }
            }
        }

        public void RearrangeIndices()
        {
            var instructions = AllCodeLines;

            for (int i = 0; i < instructions.Count; i++)
            {
                var codeLine = instructions[i];
                codeLine.InstructionIndex = i+1;
            }
        }

        public void RearrangePositions(GameObject parentContainer)
        {
            GameObject parentInstruction = null;

            for(int i=0; i<parentContainer.transform.childCount; i++)
            {
                var child = parentContainer.transform.GetChild(i);
                if (child.tag == "Instruction" || parentContainer.name == "SolutionPanel")
                {
                    parentInstruction = child.gameObject;
                    break;
                }
            }

            if (parentInstruction == null)
            {
                throw new Exception($"Tried to RearrangePositions in null transform. ParentContainer: {parentContainer.name}");
            }

            var currentTopLeft = GetTopLeftForFirst(parentContainer);

            Debug.Log($"Rearranging in {parentInstruction.name}, it has {parentInstruction.transform.childCount} children", parentInstruction);

            for(int i=0; i < parentInstruction.transform.childCount; i++)
            {
                var currentChildContainer = parentInstruction.transform.GetChild(i).gameObject;
                var rectTransform = currentChildContainer.GetComponent<RectTransform>();
                var size = rectTransform.sizeDelta;

                var offset = new Vector2(size.x / 2, -size.y / 2);

                var codeLine = GetCorrespondingCodeLine(currentChildContainer);

                if (codeLine == null) continue; //FillerInstruction has no codeline

                rectTransform.anchoredPosition = currentTopLeft + offset;

                foreach(var child in codeLine.children)
                {
                    RearrangePositions(currentChildContainer);
                }

                currentTopLeft -= new Vector2(0, codeLine.BlockHeight);
                if (InstructionHelper.IsGroupInstruction(codeLine.instruction))
                {
                    currentTopLeft -= new Vector2(0, 10);
                }
            }
        }

        public void RearrangePositions(List<CodeLine> lines, Vector2 topLeft)
        {
            if (lines.Count == 0) return;
            var parentContainer = lines[0].container.transform.parent.parent;
            var currentTopLeft = topLeft;// GetTopLeftForFirst(parentContainer.gameObject);

            for (int i = 0; i < lines.Count; i++)
            {
                var currentChildContainer = lines[i].container;
                var rectTransform = currentChildContainer.GetComponent<RectTransform>();
                var size = rectTransform.sizeDelta;

                var offset = new Vector2(size.x / 2, -size.y / 2);

                var currentLine = lines[i];
                if (InstructionHelper.IsGroupInstruction(currentLine.instruction))
                {

                }

                //var oldPosition = rectTransform.anchoredPosition;

                rectTransform.anchoredPosition = currentTopLeft + offset;

                //var newPosition = rectTransform.anchoredPosition;
                //var instructionRT = lines[i].instruction.GetComponent<RectTransform>();
                //instructionRT.anchoredPosition -= newPosition - oldPosition;

                RearrangePositions(lines[i].children, FirstChildOffset);

                currentTopLeft -= new Vector2(0, currentLine.YSpacing);
                if (InstructionHelper.IsGroupInstruction(currentLine.instruction))
                {
                    
                }
            }
        }

        Vector2 FirstChildOffset => new Vector2(-100, -30);

        Transform UnpinnedCodeLineParentTransform => GameObject.Find("UIPanel").transform;
        CodeLine oldUnpinnedCodeLineParent = null;

        public void Unpin(CodeLine codeLine)
        {
            unpinnedCodeline = codeLine;
            oldUnpinnedCodeLineParent = unpinnedCodeline.parent;
            codeLine.SetParent(null);
            RemoveCodeLineFromSolution(codeLine);
            codeLine.container.transform.SetParent(UnpinnedCodeLineParentTransform);
            codeLine.instruction.transform.SetParent(codeLine.container.transform);
            //Rearrange();
        }

        public void Pin()
        {
            unpinnedCodeline = null;
        }

        public void Remove(GameObject draggedObject)
        {
            if (unpinnedCodeline != null)
            {
                RemovePairedJumpInstructionIfNeeded(unpinnedCodeline);
                RemoveUnpinned();
            }

            else
            {
                Destroy(draggedObject);
            }
        }

        public void RemoveUnpinned()
        {
            var codeLine = unpinnedCodeline;
            DestroyCodeLine(unpinnedCodeline);
            unpinnedCodeline = null;
            //Rearrange();
            RemoveDeadJumpInstructions();
        }

        public void DestroyCodeLine(CodeLine line)
        {
            DestroyImmediate(line.instruction);
            DestroyImmediate(line.container);
        }

        public void RemovePairedJumpInstructionIfNeeded(CodeLine line)
        {
            if (!InstructionHelper.IsJumpInstruction(line.instruction)) return;
            var paired = line.instruction.GetComponent<JumpInstructionScript>().bindedInstruction;
            var pairedInstruction = GetCorrespondingCodeLine(paired);
            if (pairedInstruction == null) return;
            pairedInstruction.SetParent(null);
            RemoveCodeLineFromSolution(pairedInstruction);
            Destroy(pairedInstruction.instruction);
            Destroy(pairedInstruction.container);
        }

        public void RemoveCodeLineFromSolution(CodeLine codeLine)
        {            
            Solution.Remove(codeLine);
            codeLine.SetParent(null);
        }
    }  
}

