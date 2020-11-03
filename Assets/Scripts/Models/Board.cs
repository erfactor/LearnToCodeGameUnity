using System.Collections.Generic;
using Enumerations;
using UnityEngine;

namespace Models
{
    public class Board
    {
        private Field[,] _fields;
        private List<Bot> _bots = new List<Bot>();
        public List<Bot> Bots => _bots;

        public Board(int width, int height)
        {
            _fields = new Field[width, height];
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                this[i, j] = new Field(TileType.Hole);
            }
        }

        public Field this[int x, int y]
        {
            get => _fields[x, y];
            set
            {
                if (value.Bot != null)
                {
                    _bots.Add(value.Bot);
                }
                _fields[x, y] = value;
            }
        }

        public Field this[Vector2Int location]
        {
            get => _fields[location.x, location.y];
            set => _fields[location.x, location.y] = value;
        }
    }
}