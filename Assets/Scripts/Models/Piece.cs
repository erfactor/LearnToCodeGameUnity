using UnityEngine;

namespace Models
{
    public class Piece
    {
        public Piece(Vector2Int location, int number, Transform pieceTransform)
        {
            Location = location;
            Number = number;
            _pieceTransform = pieceTransform;
        }

        public Vector2Int Location;
        public int Number;
        public readonly Transform _pieceTransform;
    }
}