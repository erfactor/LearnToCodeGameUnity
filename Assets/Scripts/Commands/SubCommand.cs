using Models;

namespace Commands
{
    public class SubCommand : ICommand
    {
        public SubCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            var boardPiece = board[bot.BoardLocation].Piece;
            
            if (bot.Piece != null && boardPiece != null)
            {
                bot.Animator.Add(boardPiece, bot.Piece, false);
            }

            return NextCommandId;
        }

        public int NextCommandId { get; }
    }
}