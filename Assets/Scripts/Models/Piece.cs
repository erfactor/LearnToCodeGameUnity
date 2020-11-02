using UnityEngine;

namespace Models
{
    public class Piece
    {
        public Piece(Vector2Int location, int number)
        {
            Location = location;
            Number = number;
        }

        public Vector2Int Location;
        public int Number;
        
    }
}