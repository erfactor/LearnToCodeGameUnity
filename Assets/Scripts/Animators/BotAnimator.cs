using Models;
using UnityEngine;

namespace Animators
{
    public class BotAnimator : MonoBehaviour
    {
        public Animator animator;
        private static readonly int PickPiece = Animator.StringToHash("PickPiece");
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

        private Piece _pickedPiece = null;
        public void Pick(Piece piece)
        {
            _pickedPiece = piece;
            animator.SetTrigger(PickPiece);
        }

        public void AttachPieceToHand()
        {
            var pieceTransform = _pickedPiece._pieceTransform;
            var leftArm = GetComponent<Transform>().Find("LeftArmSolver").Find("LeftArmSolver_Target");
            pieceTransform.parent = leftArm;
        }
    }
}
