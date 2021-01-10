using Models;

namespace Commands
{
    public class IncCommand : ICommand
    {
        public int NextCommandId { get; }

        public float ExecutionTime { get; } = 1.2f;

        public IncCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            if (bot.Piece != null)
            {
                bot.Animator.Inc(bot.Piece, true);
            }

            return NextCommandId;
        }
    }
}