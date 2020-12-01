using Models;
using UnityEngine;

namespace Animators
{
    public class BotAnimator : MonoBehaviour
    {
        public Animator animator;
        private static readonly int PickPiece = Animator.StringToHash("PickPiece");
        private static readonly int PutPiece = Animator.StringToHash("PutPiece");
        public Bot Bot { get; set; }

        void Update()
        {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if(Input.GetKeyDown(KeyCode.UpArrow))
                animator.SetTrigger("Up");
            if(Input.GetKeyDown(KeyCode.DownArrow))
                animator.SetTrigger("Down");
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                animator.SetTrigger("Left");
            if(Input.GetKeyDown(KeyCode.RightArrow))
                animator.SetTrigger("Right");
        }

        public void Move(Direction direction)
        {
            animator.SetTrigger(direction.ToString());
        }

        private Piece _piece = null;
        private bool _isPicking;
        public void Pick(Piece piece)
        {
            _isPicking = true;
            _piece = piece;
            animator.SetTrigger(PickPiece);
        }
        
        public void Put(Piece piece)
        {
            _isPicking = false;
            _piece = piece;
            animator.SetTrigger(PutPiece);
        }

        public void AttachPieceToHand()
        {
            var pieceTransform = _piece._pieceTransform;
            var leftArm = GetComponent<Transform>().Find("LeftArmSolver").Find("LeftArmSolver_Target");
            pieceTransform.parent = _isPicking ? leftArm : null;
        }
    }
}
