using Models;

namespace Commands
{
    public class FinishCommand : ICommand
    {
        public int Execute(Board board, Bot bot)
        {
            return NextCommandId;
        }

        public int NextCommandId { get; } = -1;

        public float ExecutionTime { get; } = 0.0f;
    }
}