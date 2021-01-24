using Enumerations;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    public class LogicalExpression
    {
        public List<Condition> conditions;
        public List<LogicalOperator> logicalOperators;

        public bool Evaluate(Board board, Bot bot)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                bool subResult = conditions[i].Evaluate(board, bot);

                if (i == logicalOperators.Count) //if it is the last condition
                {
                    return subResult;
                }

                if (logicalOperators[i] == LogicalOperator.And)
                {
                    if (subResult == false) return false; //if the operator is AND and leftSide == false, there is no way the whole expression will ever be true
                }

                if (logicalOperators[i] == LogicalOperator.Or)
                {
                    if (subResult == true) return true; //if the operator is OR and leftSide == true, there is no way the whole expression will ever be false
                }
            }

            throw new NotImplementedException();
        }
    }

    public class Condition
    {
        public static readonly Dictionary<Direction, Vector2Int> directionMapping = new Dictionary<Direction, Vector2Int>()
        {
            { Direction.UpLeft, new Vector2Int(-1, 1)},
            { Direction.Up, new Vector2Int(0, 1)},
            { Direction.UpRight, new Vector2Int(1, 1)},
            { Direction.Left, new Vector2Int(-1, 0)},
            { Direction.Center, new Vector2Int(0, 0)},
            { Direction.Right, new Vector2Int(1, 0)},
            { Direction.DownLeft, new Vector2Int(-1, -1)},
            { Direction.Down, new Vector2Int(0, -1)},
            { Direction.DownRight, new Vector2Int(1, -1)}
        };

        public Direction leftSide;
        public ComparisonSign comparisonSign;
        public ComparisonObject rightSide;

        public bool Evaluate(Board board, Bot bot)
        {
            var leftSideLocation = bot.BoardLocation + directionMapping[leftSide];
            bot.Animator.CreateIfIndicator(leftSideLocation, bot.Color);
            if (!board.IsInside(leftSideLocation)) return false;

            var leftSideField = board[leftSideLocation];

            switch (rightSide.comparedType)
            {
                case ComparedType.Direction:
                    {
                        var rightSideLocation = bot.BoardLocation + directionMapping[rightSide.comparedDirection];
                        var rightSideField = board[rightSideLocation];
                        return CompareWithDirection(leftSideField, rightSideField, comparisonSign);
                    }

                case ComparedType.Something:
                    {
                        return CompareWithSomething(leftSideField, comparisonSign);
                    }

                case ComparedType.Floor:
                    {
                        return CompareWithFloor(leftSideField, comparisonSign);
                    }

                case ComparedType.Item:
                    {
                        return CompareWithItem(leftSideField, comparisonSign);
                    }
                case ComparedType.Hole:
                    {
                        return CompareWithHole(leftSideField, comparisonSign);
                    }
                case ComparedType.Bot:
                    {
                        return CompareWithBot(leftSideField, comparisonSign);
                    }
                case ComparedType.Wall:
                    {
                        return CompareWithWall(leftSideField, comparisonSign);
                    }
                case ComparedType.Number:
                    {
                        return CompareWithNumber(leftSideField, comparisonSign, rightSide.numberValue);
                    }

                default:
                    throw new ArgumentException();
            }
        }

        public bool IsEqualitySign(ComparisonSign comparisonSign)
        {
            return comparisonSign == ComparisonSign.Equal || comparisonSign == ComparisonSign.NotEqual;
        }

        public bool IsRelationSign(ComparisonSign comparisonSign)
        {
            return !IsEqualitySign(comparisonSign);
        }

        public bool CompareWithDirection(Field leftSideField, Field rightSideField, ComparisonSign comparisonSign)
        {
            throw new System.NotImplementedException();
        }

        public bool CompareWithSomething(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > something?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return CompareWithFloor(leftSideField, ComparisonSign.NotEqual);
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return CompareWithFloor(leftSideField, ComparisonSign.Equal);
            }

            throw new ArgumentException();
        }

        public bool CompareWithFloor(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > nothing?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.IsEmptyFloor;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return !leftSideField.IsEmptyFloor;
            }

            throw new ArgumentException();
        }

        public bool CompareWithItem(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > item?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.HasItem;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return !leftSideField.HasItem;
            }

            throw new ArgumentException();
        }

        public bool CompareWithHole(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > hole?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.TileType == TileType.Hole;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return leftSideField.TileType != TileType.Hole;
            }

            throw new ArgumentException();
        }

        public bool CompareWithBot(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > bot?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.Bot != null;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return leftSideField.Bot == null;
            }

            throw new ArgumentException();
        }

        public bool CompareWithWall(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > wall?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.TileType == TileType.Wall;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return leftSideField.TileType != TileType.Wall;
            }

            throw new ArgumentException();
        }

        public bool CompareWithNumber(Field leftSideField, ComparisonSign comparisonSign, int comparedValue)
        {
            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.Piece.Number == comparedValue;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return leftSideField.Piece.Number != comparedValue;
            }

            if (comparisonSign == ComparisonSign.Greater)
            {
                return leftSideField.Piece.Number > comparedValue;
            }

            if (comparisonSign == ComparisonSign.GreaterEqual)
            {
                return leftSideField.Piece.Number >= comparedValue;
            }

            if (comparisonSign == ComparisonSign.Lesser)
            {
                return leftSideField.Piece.Number < comparedValue;
            }

            if (comparisonSign == ComparisonSign.LesserEqual)
            {
                return leftSideField.Piece.Number <= comparedValue;
            }

            throw new ArgumentException();
        }
    }

    public class ComparisonObject
    {
        public ComparedType comparedType;
        public int numberValue;
        public Direction comparedDirection;
    }

    public enum ComparedType
    {
        Direction,
        Something,
        Floor,
        Number,
        Item,
        Bot,
        Wall,
        Hole
    }

    public enum ComparisonSign
    {
        Equal,
        NotEqual,
        Greater,
        Lesser,
        GreaterEqual,
        LesserEqual
    }

    public enum LogicalOperator
    {
        Or,
        And
    }
}
