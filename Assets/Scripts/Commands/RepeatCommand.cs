using System;
using System.Collections.Generic;
using Enumerations;
using Models;
using UnityEngine;

namespace Commands
{
    public class RepeatCommand : ICommand
    {
        public int NextCommandId { get; set; }
        public int TrueCommandId { get; set; }

        public float ExecutionTime { get; } = 0.0f;

        public LogicalExpression logicalExpression;

        private int counter;

        public RepeatCommand(int trueCommandId, int falseCommandId, int times)
        {
            NextCommandId = falseCommandId;
            TrueCommandId = trueCommandId;
            counter = times;
        }        

        public int Execute(Board board, Bot bot)
        {
            counter--;

            if (counter >= 0)
            {
                return TrueCommandId;
            }
            else
            {
                return NextCommandId;
            }
        }        
    }   
}