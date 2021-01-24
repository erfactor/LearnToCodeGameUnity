using Enumerations;

namespace Models
{
    public class Field
    {
        public TileType TileType;
        public Bot Bot;
        public Piece Piece;

        public Field(TileType tileType, Bot bot = null, Piece piece = null)
        {
            TileType = tileType;
            Bot = bot;
            Piece = piece;
        }

        public bool IsEmptyFloor
        {
            get
            {
                return Bot == null && Piece == null && TileType == TileType.Normal;
            }
        }

        public bool HasItem => Piece != null;
    }
}