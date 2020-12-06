using Models;

namespace Commands
{
    public class DecCommand : ICommand
    {
        public DecCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            if (bot.Piece != null)
            {
                bot.Animator.Inc(bot.Piece, false);
            }

            return NextCommandId;
        }

        public int NextCommandId { get; }
    }
}