using Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CommandHelper
    {
        public ICommand GetCommand(List<CodeLine> solution, int currentLine)
        {
            var line = solution[currentLine];

            if (InstructionHelper.IsMoveInstruction(line.instruction))
            {
                return new MoveCommand(InstructionHelper.GetInstructionDirection(line.instruction), currentLine + 1);
            }

            if (InstructionHelper.IsPickInstruction(line.instruction))
            {
                return new PickCommand(currentLine + 1);
            }

            if (InstructionHelper.IsPutInstruction(line.instruction))
            {
                return new PutCommand(currentLine + 1);
            }

            if (InstructionHelper.IsAddInstruction(line.instruction))
            {
                return new AddCommand(currentLine + 1);
            }

            if (InstructionHelper.IsSubInstruction(line.instruction))
            {
                return new SubCommand(currentLine + 1);
            }

            if (InstructionHelper.IsDecInstruction(line.instruction))
            {
                return new DecCommand(currentLine + 1);
            }

            if (InstructionHelper.IsIncInstruction(line.instruction))
            {
                return new IncCommand(currentLine + 1);
            }

            if (InstructionHelper.IsJumpInstruction(line.instruction))
            {
                if (InstructionHelper.IsJumpInstructionLabel(line.instruction))
                {
                    return new JumpCommand(currentLine + 1);
                }

                int jumpIndex = GetJumpLineNumber(solution, line.instruction);
                return new JumpCommand(jumpIndex);
            }

            if (InstructionHelper.IsIfInstruction(line.instruction))
            {
                int trueLineNumber = currentLine + 1;
                int elseLineNumber = currentLine + line.TotalChildrenCount + 1;
                return new IfCommand(trueLineNumber, elseLineNumber, GetConditions(line), GetLogicalOperators(line));
            }

            throw new System.Exception("Could not generate a command.");
        }

        public List<Condition> GetConditions(CodeLine ifLine)
        {
            var directionIndicator = ifLine.instruction.transform.Find("DirectionIndicator");
            var comparisonIndicator = ifLine.instruction.transform.Find("ComparisonIndicator");
            var relationIndicator = ifLine.instruction.transform.Find("Dropdown").Find("Label");
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
                if (go == solution[i].instruction) return i;
            }

            return -1;
        }
    }

}
