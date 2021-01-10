using Models;

namespace Commands
{
    public class AddCommand : ICommand
    {
        public int NextCommandId { get; }

        public float ExecutionTime { get; } = 1.2f;

        public AddCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            var boardPiece = board[bot.BoardLocation].Piece;
            
            if (bot.Piece != null && boardPiece != null)
            {
                bot.Animator.Add(boardPiece, bot.Piece, true);
            }

            return NextCommandId;
        }

    }
}