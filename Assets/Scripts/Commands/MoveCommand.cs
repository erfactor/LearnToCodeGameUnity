using System;
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

        public MoveCommand(Direction direction, int nextCommandId)
        {
            _direction = direction;
            NextCommandId = nextCommandId;
        }

        public int NextCommandId { get; }

        public int Execute(Board board, Bot bot)
        {
            var newLocation = bot.BoardLocation + _directionVector[_direction];
            var newLocationField = board[newLocation];
            if (newLocationField?.TileType == TileType.Normal || newLocationField.Bot == null)
            {
                Console.WriteLine(newLocationField.TileType);
                board[bot.BoardLocation].Bot = null;
                bot.BoardLocation = newLocation;
                newLocationField.Bot = bot;
                bot.Animator.ExecuteMove(_direction);
            }

            return NextCommandId;
        }
    }
}