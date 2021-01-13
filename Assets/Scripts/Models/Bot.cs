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
        public bool HasFinished { get; set; }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Animator.transform.Find("Body").GetComponent<SpriteRenderer>().color = _color;
            }
        }

        public Bot(Vector2Int boardLocation, BotAnimator animator)
        {
            BoardLocation = boardLocation;
            Animator = animator;
        }
    }
}