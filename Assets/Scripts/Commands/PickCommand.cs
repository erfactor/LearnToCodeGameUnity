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
            var boardPiece = board[bot.BoardLocation].Piece;
            if (bot.Piece == null && boardPiece != null)
            {
                bot.Animator.Pick(boardPiece);
                bot.Piece = boardPiece;
                board[bot.BoardLocation].Piece = null;
            }

            return NextCommandId;
        }

        public int NextCommandId { get; }
    }
}