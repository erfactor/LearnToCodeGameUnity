using Models;

namespace Commands
{
    public class PutCommand : ICommand
    {
        public int NextCommandId { get; }
        public float ExecutionTime { get; } = 1.2f;

        public PutCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            var boardPiece = board[bot.BoardLocation].Piece;
            
            if (bot.Piece != null && boardPiece == null)
            {
                bot.Animator.Put(bot.Piece);
                board[bot.BoardLocation].Piece = bot.Piece;
                bot.Piece = null;
            }

            return NextCommandId;
        }

        
    }
}