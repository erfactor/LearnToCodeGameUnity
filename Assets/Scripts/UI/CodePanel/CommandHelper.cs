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

        private int currentCodeLineNumber;
        public Dictionary<ICommand, CodeLine> commandToCodeLineMapping;
        private List<ICommand> allCommands;

        public List<ICommand> GetCommands(List<CodeLine> rootContainerCodeLines)
        {           
            currentCodeLineNumber = 0;
            commandToCodeLineMapping = new Dictionary<ICommand, CodeLine>();
            allCommands = new List<ICommand>();

            foreach(var codeLine in rootContainerCodeLines)
            {
                GenerateCodeForInstruction(codeLine);
            }

            RepairJumps();
            allCommands.Add(new FinishCommand());
            return allCommands;
        }

        public void GenerateCodeForInstruction(CodeLine line)
        {
            if (InstructionHelper.IsMoveInstruction(line.instruction))
            {
                var command = new MoveCommand(InstructionHelper.GetInstructionDirection(line.instruction), currentCodeLineNumber + 1);
                commandToCodeLineMapping.Add(command, line);
                allCommands.Add(command);
            }
            if (InstructionHelper.IsPutInstruction(line.instruction))
            {
                var command = new PutCommand(currentCodeLineNumber + 1);
                commandToCodeLineMapping.Add(command, line);
                allCommands.Add(command);
            }
            if (InstructionHelper.IsPickInstruction(line.instruction))
            {
                var command = new PickCommand(currentCodeLineNumber + 1);
                commandToCodeLineMapping.Add(command, line);
                allCommands.Add(command);
            }
            if (InstructionHelper.IsJumpInstruction(line.instruction))
            {
                ICommand command;

                if (InstructionHelper.IsJumpInstructionLabel(line.instruction))
                {
                    command = new JumpCommand(currentCodeLineNumber + 1);
                    allCommands.Add(command);
                }
                else
                {
                    //this is being set in code later - otherwise forward jumps will not work - see RepairJumps for reference.
                    command = new JumpCommand(currentCodeLineNumber + 1);
                    allCommands.Add(command);
                }
                commandToCodeLineMapping.Add(command, line);
            }

            if (InstructionHelper.IsIfInstruction(line.instruction))
            {
                int trueLineNumber = currentCodeLineNumber + 1;
                int elseLineNumber = currentCodeLineNumber + line.TotalChildrenCount + 1;
                var command = new IfCommand(trueLineNumber, elseLineNumber, GetConditions(line), GetLogicalOperators(line));
                allCommands.Add(command);
                commandToCodeLineMapping.Add(command, line);
                currentCodeLineNumber++;
                foreach (var child in line.children)
                {
                    GenerateCodeForInstruction(child);
                }
                command.NextCommandId = currentCodeLineNumber;
                currentCodeLineNumber--; //this may seem wrong but actually it is not
            }

            if (InstructionHelper.IsWhileInstruction(line.instruction))
            {
                int trueLineNumber = currentCodeLineNumber + 1;
                int falseLineNumber = currentCodeLineNumber + line.TotalChildrenCount + 1;
                var command = new WhileCommand(trueLineNumber, falseLineNumber, GetConditions(line), GetLogicalOperators(line));
                allCommands.Add(command);
                commandToCodeLineMapping.Add(command, line);
                currentCodeLineNumber++;
                foreach (var child in line.children)
                {
                    GenerateCodeForInstruction(child);
                }
                var loopJumpCommand = new JumpCommand(trueLineNumber - 1);
                commandToCodeLineMapping.Add(loopJumpCommand, null);
                command.NextCommandId = currentCodeLineNumber+1;
                allCommands.Add(loopJumpCommand);
            }

            if (InstructionHelper.IsRepeatInstruction(line.instruction))
            {
                int trueLineNumber = currentCodeLineNumber + 1;
                int falseLineNumber = currentCodeLineNumber + line.TotalChildrenCount + 1;
                var command = new RepeatCommand(trueLineNumber, falseLineNumber, InstructionHelper.GetRepeatTimes(line.instruction));
                allCommands.Add(command);
                commandToCodeLineMapping.Add(command, line);
                currentCodeLineNumber++;
                foreach (var child in line.children)
                {
                    GenerateCodeForInstruction(child);
                }
                var loopJumpCommand = new JumpCommand(trueLineNumber - 1);
                commandToCodeLineMapping.Add(loopJumpCommand, null);
                command.NextCommandId = currentCodeLineNumber;                
                allCommands.Add(loopJumpCommand);
            }

            currentCodeLineNumber++;
        }
        private void RepairJumps()
        {
            for(int i=0; i<allCommands.Count; i++)
            {
                var currentCommand = allCommands[i];
                if (!(currentCommand is JumpCommand)) continue;
                var jumpCommand = currentCommand as JumpCommand;
                var jumpCodeLine = commandToCodeLineMapping[jumpCommand];
                if (jumpCodeLine == null) continue;
                if (InstructionHelper.IsJumpInstructionLabel(jumpCodeLine.instruction)) continue; //labels does not need to be repaired                
                var pairedLine = GetPairedLabelCommand(jumpCodeLine);
                var indexToJump = allCommands.IndexOf(pairedLine);
                jumpCommand.NextCommandId = indexToJump;
            }
        }

        private ICommand GetPairedLabelCommand(CodeLine mainJumpLine)
        {
            var jumpScript = mainJumpLine.instruction.GetComponent<JumpInstructionScript>();
            var paired = jumpScript.bindedInstruction;
            var pairedCodeLine = commandToCodeLineMapping.Where(x => x.Value.instruction == paired).First();
            return pairedCodeLine.Key;
        }
    }

}
