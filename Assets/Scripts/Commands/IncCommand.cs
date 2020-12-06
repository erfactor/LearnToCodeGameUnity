using Models;

namespace Commands
{
    public class IncCommand : ICommand
    {
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

        public int NextCommandId { get; }
    }
}