using Models;

namespace Commands
{
    public class PickCommand : ICommand
    {
        public int NextCommandId { get; }

        public float ExecutionTime { get; } = 1.2f;

        public PickCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            var boardPiece = board[bot.BoardLocation].Piece;
            if (bot.Piece == null && boardPiece != null)
            {
                bot.Animator.Pick(boardPiece);
                bot.Piece = boardPiece;
                board[bot.BoardLocation].Piece = null;
            }

            return NextCommandId;
        }
    }
}