using System;
using System.Collections.Generic;
using Enumerations;
using Models;
using UnityEngine;

namespace Commands
{
    public class IfCommand : ICommand
    {
        public int NextCommandId { get; }
        public int TrueCommandId { get; set; }

        public float ExecutionTime { get; } = 0.0f;

        List<Condition> conditions;
        List<LogicalOperator> logicalOperators;
        

        private readonly Dictionary<Direction, Vector2Int> _directionVector = new Dictionary<Direction, Vector2Int>
        {
            {Direction.Left, Vector2Int.left},
            {Direction.Down, Vector2Int.down},
            {Direction.Right, Vector2Int.right},
            {Direction.Up, Vector2Int.up}
        };

        public IfCommand(int trueCommandId, int falseCommandId, List<Condition> conditions, List<LogicalOperator> logicalOperators)
        {
            NextCommandId = falseCommandId;
            TrueCommandId = trueCommandId;
            this.conditions = conditions;
            this.logicalOperators = logicalOperators;
        }

        

        public int Execute(Board board, Bot bot)
        {
            bool evaluationResult = Evaluate(board, bot);

            if (evaluationResult)
            {
                return TrueCommandId;
            }
            else
            {
                return NextCommandId;
            }
        }

        public bool Evaluate(Board board, Bot bot)
        {
            for(int i=0; i<conditions.Count; i++)
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
            bot.Animator.CreateIfIndicator(leftSideLocation);
            if (!board.IsInside(leftSideLocation)) return false;

            var leftSideField = board[leftSideLocation];

            switch(rightSide.comparedType)
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

                case ComparedType.Nothing:
                    {
                        return CompareWithNothing(leftSideField, comparisonSign);
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
                return CompareWithNothing(leftSideField, ComparisonSign.NotEqual);
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return CompareWithNothing(leftSideField, ComparisonSign.Equal);
            }

            throw new ArgumentException();
        }

        public bool CompareWithNothing(Field leftSideField, ComparisonSign comparisonSign)
        {
            if (IsRelationSign(comparisonSign)) return false; //how can we say left > nothing?

            if (comparisonSign == ComparisonSign.Equal)
            {
                return leftSideField.IsNothing;
            }

            if (comparisonSign == ComparisonSign.NotEqual)
            {
                return !leftSideField.IsNothing;
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
        Nothing,
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