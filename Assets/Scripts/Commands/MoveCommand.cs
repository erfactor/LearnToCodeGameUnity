using Enumerations;
using Models;

namespace Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Direction _direction;
        public int NextCommandId { get; }

        public MoveCommand(Direction direction, int nextCommandId)
        {
            _direction = direction;
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            bot.Animator.ExecuteMove(_direction);

            return NextCommandId;
        }

    }
}