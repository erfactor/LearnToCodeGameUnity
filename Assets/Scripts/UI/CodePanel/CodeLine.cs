using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Commands;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CodeLine
    {
        public const float DefaultContainerHeight = 60;

        public const float MarginX = 40;
        public static GameObject CodeLineNumberIndicatorPrefab => GameObject.Find("CodeLineNumberIndicatorPrefab");
        public static GameObject NestPrefab => GameObject.Find("NestPrefab");

        public Vector2 BlockSize
        {
            get
            {
                var containerRT = container.GetComponent<RectTransform>();
                var instructionRT = instruction.GetComponent<RectTransform>();
                var rect = containerRT.rect;
                return rect.size;
            }
        }

        private int _instructionIndex;
        public int InstructionIndex
        {
            get
            {
                return _instructionIndex;
            }
            set
            {
                _instructionIndex = value;
                lineNumberTextObject.GetComponent<Text>().text = (_instructionIndex).ToString().PadLeft(2, '0');
            }
        }

        public CodeLine parent;

        public GameObject container;
        public GameObject instruction;
        public GameObject lineNumberTextObject;
        public GameObject nestObject;

        public List<CodeLine> children;

        public List<CodeLine> AllChildrenInHierarchy
        { 
            get
            {
                List<CodeLine> allChildren = new List<CodeLine>();
                for(int i=0; i<children.Count; i++)
                {
                    allChildren.Add(children[i]);
                    allChildren.AddRange(children[i].AllChildrenInHierarchy);
                }

                return allChildren;
            }
            
        }

        public int ChildrenCount => children.Count;

        public int TotalChildrenCount => ChildrenCount + children.Sum(x => x.ChildrenCount);

        private CodeLine _temporaryCodeLine;
        public CodeLine TemporaryCodeLine
        {
            get
            {
                return _temporaryCodeLine;
            }
            set
            {
                if (parent != null)
                {
                    parent.TemporaryCodeLine = value;
                }
                _temporaryCodeLine = value;
            }
        }

        public float TemporaryCodeLineHeight => TemporaryCodeLine != null ? TemporaryCodeLine.BlockHeight : 0;
        public const float NestWidth = 300;

        public float _desiredNestHeight = 30;
        public float DesiredNestHeight => _desiredNestHeight + TemporaryCodeLineHeight;

        public float RealtimeNestHeight => nestObject.GetComponent<RectTransform>().rect.height;
        
        private float _blockHeight = 60.0f;
        public float BlockHeight
        {
            get
            {
                return _blockHeight;                
            }
        }

        public float YSpacing
        {
            get
            {
                if (InstructionHelper.IsGroupInstruction(instruction))
                {
                    return 30 + RealtimeNestHeight;
                }
                else
                {
                    return 60;
                }
            }
        }

        public float ChildrenHeight
        {
            get
            {
                if (InstructionHelper.IsGroupInstruction(instruction))
                {
                    return 60 + children.Sum(x => x.ChildrenHeight);
                }
                else return 60;
            }
        }

        public CodeLine(GameObject container, GameObject instruction)
        {
            this.instruction = instruction;
            this.container = container;
            instruction.transform.SetParent(container.transform);
            instruction.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            children = new List<CodeLine>();
            AddTextComponent();
            instruction.transform.SetAsFirstSibling();

            AddNestObjectComponentIfNeeded();

        }

        public void AddTextComponent()
        {
            lineNumberTextObject = GameObject.Instantiate(CodeLineNumberIndicatorPrefab);
            lineNumberTextObject.transform.SetParent(container.transform);
        }

        public void AddNestObjectComponentIfNeeded()
        {
            if (!InstructionHelper.IsGroupInstruction(instruction)) return;
            nestObject = GameObject.Instantiate(NestPrefab);
            nestObject.GetComponent<Image>().color = instruction.GetComponent<Image>().color;
            nestObject.transform.SetParent(container.transform);
            nestObject.transform.SetAsFirstSibling();
        }

        public void AddChild(CodeLine line, int siblingIndex)
        {
            if (children.Contains(line))
            {
                Debug.LogWarning($"Tried to add children with index {line.InstructionIndex} to parent with index {InstructionIndex}, but it was already in a children list.");
            }
            else
            {
                children.Insert(siblingIndex, line);
                line.container.transform.SetParent(instruction.transform);
                ChangeDesiredNestHeight(+line.BlockSize.y);
                
                ChangeBlockSize(line.BlockHeight);
            }
        }

        public void RemoveChild(CodeLine line)
        {
            if (!children.Contains(line))
            {
                Debug.LogWarning($"Tried to remove children with index {line.InstructionIndex} from parent with index {InstructionIndex}, but it was NOT in a children list.");
            }
            else
            {
                children.Remove(line);
                ChangeDesiredNestHeight(-line.BlockSize.y);
                ChangeBlockSize(-line.BlockHeight);
            }
        }

        public void ChangeBlockSize(float change)
        {
            _blockHeight += change;
            if (parent != null)
            {
                parent.ChangeBlockSize(change);
            }
        }

        public Vector2 GetTopLeftPositionForFirstContainer()
        {
            float posX = instruction.GetComponent<RectTransform>().rect.xMin;
            float posY = -DefaultContainerHeight / 2;
            return new Vector2(posX, posY);
        }

        public void RearrangeChildren()
        {
            //var currentTopLeft = GetTopLeftPositionForFirstContainer();
            //for (int i = 0; i < children.Count; i++)
            //{
            //    var child = children[i];
            //    var childContainerRT = child.container.GetComponent<RectTransform>();
            //    var size = childContainerRT.sizeDelta;
            //    float newX = currentTopLeft.x + size.x / 2;
            //    float newY = currentTopLeft.y - size.y / 2;
            //    childContainerRT.anchoredPosition = new Vector2(newX, newY);
            //    currentTopLeft += new Vector2(0, -size.y);
            //}
        }

        public void RecalculateContainer()
        {
            //var sumHeight = DefaultContainerHeight + children.Sum(x => x.BlockSize.y);
            var containerRT = container.GetComponent<RectTransform>();
            //var containerRTrect = container.GetComponent<RectTransform>().rect;
            var oldPosition = containerRT.anchoredPosition;
            Vector2 oldSize = containerRT.sizeDelta;
            //Vector2 newSize = new Vector2(containerRT.rect.width, sumHeight);
            Vector2 newSize = new Vector2(containerRT.rect.width, BlockHeight);
            Vector2 newPosition = containerRT.anchoredPosition - (newSize - oldSize) / 2.0f;
            containerRT.sizeDelta = newSize;
            containerRT.anchoredPosition = newPosition;

            var instructionRT = instruction.GetComponent<RectTransform>();
            instructionRT.anchoredPosition -= newPosition - oldPosition;

            // Debug.Log($"new height: {sumHeight}");
        }

        public void ChangeDesiredNestHeight(float change)
        {
            _desiredNestHeight += change;
            if (parent != null)
            {
                parent.ChangeDesiredNestHeight(change);
            }
        }

        public void SetParent(CodeLine newParent, int siblingIndex = -1)
        {
            if (siblingIndex == -1)
            {
                if (newParent != null)
                {
                    siblingIndex = newParent.children.Count;
                }
            }

            if (parent != null)
            {
                parent.RemoveChild(this);
            }

            parent = newParent;

            if (parent != null)
            {
                newParent.AddChild(this, siblingIndex);
                container.transform.SetSiblingIndex(siblingIndex);
            }
        }

        #region PHYSICS

        public void BaseUpdate()
        {
            UpdateMainCodeLinePosition();
            UpdateReturnPhysics();
            UpdateNestPositionAndSize();
            UpdateArrowIfNeeded();

            RecalculateContainer();
            RearrangeChildren();
        }

        public void RepelUpdate(Vector2 mousePosition)
        {
            UpdateRepelPhysics(mousePosition);
        }

        public void UpdateNestPositionAndSize()
        {
            if (nestObject == null) return; 

            var instructionRT = instruction.GetComponent<RectTransform>();
            var nestRT = nestObject.GetComponent<RectTransform>();

            var oldHeight = nestRT.rect.height;
            var newHeight = Mathf.Lerp(oldHeight, DesiredNestHeight, 0.17f);

            nestRT.sizeDelta = new Vector2(nestRT.sizeDelta.x, newHeight);

            var newX = 0;// instructionRT.rect.xMin + nestRT.sizeDelta.x/2;
            var newY = instructionRT.anchoredPosition.y - newHeight / 2;

            nestRT.anchoredPosition = new Vector2(newX, newY);
        }

        public void UpdateMainCodeLinePosition()
        {
            var containerRT = container.GetComponent<RectTransform>();
            var instructionRT = instruction.GetComponent<RectTransform>();
            var textRT = lineNumberTextObject.GetComponent<RectTransform>();

            float instructionX = -containerRT.rect.width / 2 + instructionRT.rect.width / 2 + MarginX;
            float instructionY = instructionRT.anchoredPosition.y;
            float textX = -containerRT.rect.width / 2 + textRT.rect.width / 2;
            float textY = instructionRT.anchoredPosition.y;

            instructionRT.anchoredPosition = new Vector2(instructionX, instructionY);
            textRT.anchoredPosition = new Vector2(textX, textY);
        }

        public void UpdateArrowIfNeeded()
        {
            //if (InstructionHelper.IsJumpInstruction(instruction))
            //{
            //    if (instruction.GetComponent<JumpInstructionScript>().arrow == null) Debug.Log("Null at arrow");
            //    if (instruction.GetComponent<JumpInstructionScript>().arrow.GetComponent<JumpInstructionArrow>() == null) Debug.Log("Null at arrow script");
            //    instruction.GetComponent<JumpInstructionScript>().arrow.GetComponent<JumpInstructionArrow>().UpdateCurve();
            //}
        }

        public void UpdateReturnPhysics()
        {
            var returnForce = CalculateReturnForce();
            var instructionRT = instruction.GetComponent<RectTransform>();
            instructionRT.anchoredPosition += returnForce;
        }

        public void UpdateRepelPhysics(Vector2 mousePosition)
        {
            var repelForce = CalculateRepelForce(mousePosition);
            var instructionRT = instruction.GetComponent<RectTransform>();
            instructionRT.anchoredPosition += repelForce;
        }


        public float PhysicsCalculationYOffset => DefaultContainerHeight / 2;
        public Vector2 PositionForPhysicsCalculation
        {
            get
            {
                //var containerPosition = container.transform.position;
                var containerPosition = container.transform.position;
                var posX = containerPosition.x;
                //var posY = instruction.transform.position.y;// containerPosition.y + size.y / 2 - PhysicsCalculationYOffset;
                var posY = instruction.transform.position.y;// containerPosition.y + size.y / 2 - PhysicsCalculationYOffset;
                return new Vector2(posX, posY);
            }
        }

        public Vector2 DefaultInstructionYWithZeroForce
        {
            get
            {
                var containerRT = container.GetComponent<RectTransform>();
                var instructionRT = instruction.GetComponent<RectTransform>();
                var containerPosition = container.transform.position;
                var defaultX = 0; //assign sth else if needed;
                var defaultY = containerPosition.y + containerRT.rect.height / 2 - PhysicsCalculationYOffset;
                return new Vector2(defaultX, defaultY);
            }
        }

        private float RepelConst = 5;

        public Vector2 CalculateRepelForce(Vector2 mousePosition)
        {
            Vector2 blockPosition = PositionForPhysicsCalculation;

            if (blockPosition.y > mousePosition.y) return Vector2.zero;

            var yForce = -RepelConst;

            return new Vector2(0, yForce);
        }

        public Vector2 CalculateReturnForce()
        {
            var defaultY = DefaultInstructionYWithZeroForce.y;
            var currentY = PositionForPhysicsCalculation.y;
            var yDifference = currentY - defaultY;
            var yForce = -yDifference / (60/RepelConst);
            return new Vector2(0, yForce);
        }

        public bool IsInsideNest(Vector2 worldMousePosition)
        {
            //if (!InstructionHelper.IsGroupInstruction(instruction)) return false;

            var nestSize = nestObject.GetComponent<RectTransform>().rect.size;
            var nestPos = nestObject.transform.position;

            float x = worldMousePosition.x;
            float y = worldMousePosition.y;

            float xMin = nestPos.x - nestSize.x / 2;
            float xMax = nestPos.x + nestSize.x / 2;
            float yMin = nestPos.y - nestSize.y / 2;
            float yMax = nestPos.y + nestSize.y / 2;

            return x >= xMin && x <= xMax && y >= yMin && y <= yMax;
        }

        #endregion PHYSICS
    }
}
