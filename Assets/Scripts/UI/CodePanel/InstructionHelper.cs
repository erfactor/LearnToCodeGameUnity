using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
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

        public static bool IsGroupInstruction(GameObject go)
        {
            if (IsIfInstruction(go)) return true; 
            return false;
        }

        public static bool IsWhileInstruction(GameObject go)
        {
            throw new NotImplementedException();
        }

        public static bool IsRepeatInstruction(GameObject go)
        {
            throw new NotImplementedException();
        }

        public static Direction GetInstructionDirection(GameObject go)
        {
            Direction direction = go.transform.Find("DirectionIndicator").GetComponent<DirectionIndicatorScript>().SelectedDirection.Value;
            return direction;
        }
    }
}
