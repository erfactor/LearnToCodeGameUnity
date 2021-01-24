using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public class WhileCommand: ICommand
    {
        public int NextCommandId { get; set; }
        public int TrueCommandId { get; set; }

        public float ExecutionTime { get; } = 0.0f;

        public LogicalExpression logicalExpression;

        public WhileCommand(int trueCommandId, int falseCommandId, List<Condition> conditions, List<LogicalOperator> logicalOperators)
        {
            NextCommandId = falseCommandId;
            TrueCommandId = trueCommandId;
            logicalExpression = new LogicalExpression()
            {
                conditions = conditions,
                logicalOperators = logicalOperators
            };
        }

        public int Execute(Board board, Bot bot)
        {
            bool evaluationResult = logicalExpression.Evaluate(board, bot);

            if (evaluationResult)
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
