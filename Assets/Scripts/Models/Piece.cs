using Animators;
using UnityEngine;

namespace Models
{
    public class Piece
    {
        public Piece(Vector2Int location, int number, Transform pieceTransform)
        {
            Location = location;
            PieceTransform = pieceTransform;
            Number = number;
        }

        private Vector2Int Location;

        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                PieceTransform.GetComponent<PieceText>().Text = value.ToString();
            }
        }

        public readonly Transform PieceTransform;
        private int _number;
    }
}