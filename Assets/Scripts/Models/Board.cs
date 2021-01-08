using System.Collections.Generic;
using Enumerations;
using UnityEngine;

namespace Models
{
    public class Board
    {
        private readonly Field[,] _fields;

        public Board(int width, int height)
        {
            _fields = new Field[width, height];
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                this[i, j] = new Field(TileType.Hole);
        }

        public List<Bot> Bots
        {
            get
            {
                var result = new List<Bot>();
                for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                {
                    var bot = _fields[i, j].Bot;
                    if (bot != null) result.Add(bot);
                }

                return result;
            }
        }

        public Field this[int x, int y]
        {
            get => _fields[x, y];
            set => _fields[x, y] = value;
        }

        public Field this[Vector2Int location]
        {
            get => _fields[location.x, location.y];
            set => _fields[location.x, location.y] = value;
        }

        public int Width => _fields.GetLength(0);
        public int Height => _fields.GetLength(1);

        public bool IsInside(Vector2Int location)
        {
            return location.x < 0 || location.x >= Width || location.y < 0 || location.y >= Height;
        }
        
        
    }
}