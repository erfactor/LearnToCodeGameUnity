using Animators;
using UnityEngine;

namespace Models
{
    public class Bot
    {
        public Vector2Int BoardLocation;
        public int CommandId = 0;
        public Piece Piece { get; set; }
        public BotAnimator Animator { get; }

        public Bot(Vector2Int boardLocation, BotAnimator animator)
        {
            BoardLocation = boardLocation;
            Animator = animator;
        }
    }
}