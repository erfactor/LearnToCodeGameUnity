using Models;

namespace Commands
{
    public class JumpCommand : ICommand
    {
        private readonly Direction _direction;
        public int NextCommandId { get; set; }

        public float ExecutionTime { get; } = 0.0f;

        public JumpCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            return NextCommandId;
        }

    }
}