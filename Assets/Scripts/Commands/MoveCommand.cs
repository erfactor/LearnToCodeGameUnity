using System.Collections.Generic;
using Enumerations;
using Models;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : ICommand
    {

        private readonly Direction _direction;

        private readonly Dictionary<Direction, Vector2Int> _directionVector = new Dictionary<Direction, Vector2Int>
        {
            {Direction.Left, Vector2Int.left},
            {Direction.Down, Vector2Int.down},
            {Direction.Right, Vector2Int.right},
            {Direction.Up, Vector2Int.up}
        };

        public int NextCommandId { get; }

        public float ExecutionTime { get; } = 1.2f;

        public MoveCommand(Direction direction, int nextCommandId)
        {
            _direction = direction;
            NextCommandId = nextCommandId;
        }

        public int Execute(Board board, Bot bot)
        {
            var newLocation = bot.BoardLocation + _directionVector[_direction];
            var newLocationField = board[newLocation];
            if (newLocationField.TileType != TileType.Wall && newLocationField.Bot == null)
            {
                board[bot.BoardLocation].Bot = null;
                if (newLocationField.TileType == TileType.Normal)
                {
                    bot.BoardLocation = newLocation;
                    newLocationField.Bot = bot;
                }

                bot.Animator.Move(_direction);
                if (newLocationField.TileType == TileType.Hole)
                {
                    bot.Animator.Fall(bot);
                }
            }

            return NextCommandId;
        }
    }
}