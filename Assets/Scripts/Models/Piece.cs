using Animators;
using UnityEngine;

namespace Models
{
    public class Piece
    {
        public readonly Transform PieceTransform;
        private int _number;
        public bool isRandom;

        private Vector2Int Location;

        public Piece(Vector2Int location, int number, Transform pieceTransform, bool isRandom)
        {
            Location = location;
            PieceTransform = pieceTransform;
            Number = number;
            this.isRandom = isRandom;
        }

        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                if (PieceTransform != null) PieceTransform.GetComponent<PieceText>().Text = value.ToString();
            }
        }
    }
}