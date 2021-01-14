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
                int elseLineNumber = currentLine + line.GetAllChildrenCount() + 1;
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

}
