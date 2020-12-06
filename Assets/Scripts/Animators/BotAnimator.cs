using System;
using Models;
using UnityEngine;

namespace Animators
{
    public class BotAnimator : MonoBehaviour
    {
        public Animator animator;
        private static readonly int PickPiece = Animator.StringToHash("PickPiece");
        private static readonly int PutPiece = Animator.StringToHash("PutPiece");
        private static readonly int Add1 = Animator.StringToHash("Add");
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

        private Piece _piece;
        private Piece _heldPiece;
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
            var pieceTransform = _piece.PieceTransform;
            var leftArm = GetComponent<Transform>().Find("LeftArmSolver").Find("LeftArmSolver_Target");
            pieceTransform.parent = _isPicking ? leftArm : null;
        }
        
        public void Add(Piece piece, Piece heldPiece)
        {
            _piece = piece;
            _heldPiece = heldPiece;
            animator.SetTrigger(Add1);
        }

        public void AttachPieceToHandAdd()
        {
            var pieceTransform = _piece.PieceTransform;
            var rightArm = GetComponent<Transform>().Find("RightArmSolver").Find("RightArmSolver_Target");
            pieceTransform.parent = rightArm;
        }

        public void MergePieces()
        {
            Destroy(_piece.PieceTransform.gameObject);
            _heldPiece.Number += _piece.Number;
        }
    }
}
