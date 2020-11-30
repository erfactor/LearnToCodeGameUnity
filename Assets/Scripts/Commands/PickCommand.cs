using Models;

namespace Commands
{
    public class PickCommand : ICommand
    {
        public PickCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            if (board[bot.BoardLocation].Piece != null)
            {
                bot.Animator.Pick(board[bot.BoardLocation].Piece);
                board[bot.BoardLocation].Piece = null;
            }

            return NextCommandId;
        }

        public int NextCommandId { get; }
    }
}