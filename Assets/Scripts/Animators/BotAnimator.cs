using Models;
using UnityEngine;

namespace Animators
{
    public class BotAnimator : MonoBehaviour
    {
        private static readonly int PickPiece = Animator.StringToHash("PickPiece");
        private static readonly int PutPiece = Animator.StringToHash("PutPiece");
        private static readonly int Add1 = Animator.StringToHash("Add");
        private static readonly int Inc1 = Animator.StringToHash("Inc");
        private static readonly int Fall1 = Animator.StringToHash("Fall");
        public Animator animator;
        private Piece _heldPiece;
        private bool _isAdd;
        private bool _isInc;
        private bool _isPickingUp;

        private Piece _piece;
        private GameObject _tempPiece;
        private GameObject _fallingBot;
        private Transform _tileLevel;
        public Bot Bot { get; set; }

        private void Awake()
        {
            _tileLevel = GameObject.Find("TileLevel").transform.Find("Layer 1").transform;
        }

        public void Move(Direction direction)
        {
            animator.SetTrigger(direction.ToString());
        }

        public void Pick(Piece piece)
        {
            _isPickingUp = true;
            _piece = piece;
            animator.SetTrigger(PickPiece);
        }

        public void Put(Piece piece)
        {
            _isPickingUp = false;
            _piece = piece;
            animator.SetTrigger(PutPiece);
        }

        public void AttachPieceToHand()
        {
            var pieceTransform = _piece.PieceTransform;
            var leftArm = GetComponent<Transform>().Find("LeftArmSolver").Find("LeftArmSolver_Target");
            pieceTransform.parent = _isPickingUp ? leftArm : _tileLevel;
        }

        public void Add(Piece piece, Piece heldPiece, bool isAdd)
        {
            _isAdd = isAdd;
            _piece = piece;
            _heldPiece = heldPiece;
            animator.SetTrigger(Add1);
        }

        public void AttachPieceToHandAdd()
        {
            var rightArm = GetComponent<Transform>().Find("RightArmSolver").Find("RightArmSolver_Target");
            _tempPiece = Instantiate(_piece.PieceTransform.gameObject, rightArm, true);
        }

        public void MergePieces()
        {
            Destroy(_tempPiece);
            if (_isAdd)
                _heldPiece.Number += _piece.Number;
            else
                _heldPiece.Number -= _piece.Number;
        }

        public void Inc(Piece heldPiece, bool isInc)
        {
            _isInc = isInc;
            _heldPiece = heldPiece;
            animator.SetTrigger(Inc1);
        }

        public void AttachPieceToHandInc()
        {
            var rightArm = GetComponent<Transform>().Find("RightArmSolver").Find("RightArmSolver_Target");
            _tempPiece = Instantiate(_heldPiece.PieceTransform.gameObject, rightArm, true);
            var position = _tempPiece.transform.position;
            position.y += 0.636f * 100;
            _tempPiece.transform.position = position;
            _tempPiece.GetComponent<PieceText>().Text = _isInc ? "+" : "-";
        }

        public void MergePiecesInc()
        {
            Destroy(_tempPiece);
            if (_isInc)
                _heldPiece.Number++;
            else
                _heldPiece.Number--;
        }

        public void Fall(Bot bot)
        {
            _fallingBot = bot.Animator.gameObject;
            animator.SetBool(Fall1, true);
        }

        public void DestroyFallenBot()
        {
            Destroy(_fallingBot);
        }

        public void CreateIfIndicator(Vector2Int location, Color color)
        {
            var prefab = Resources.Load<Transform>("Bot/IfIndicator/IfIndicator");
            prefab.transform.position = new Vector3(location.x, location.y);
            var ifIndicator = Instantiate(prefab, _tileLevel, false);
            ifIndicator.GetComponent<SpriteRenderer>().color = color;
        }
    }
}