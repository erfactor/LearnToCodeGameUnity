using Models;

namespace Commands
{
    public class AddCommand : ICommand
    {
        public AddCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            var boardPiece = board[bot.BoardLocation].Piece;
            
            if (bot.Piece != null && boardPiece != null)
            {
                bot.Animator.Add(boardPiece, bot.Piece);
                board[bot.BoardLocation].Piece = null;
            }

            return NextCommandId;
        }

        public int NextCommandId { get; }
    }
}