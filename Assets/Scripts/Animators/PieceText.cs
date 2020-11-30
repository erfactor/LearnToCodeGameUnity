using UnityEngine;
using UnityEngine.UI;

namespace Animators
{
    public class PieceText : MonoBehaviour
    {
        private Text _text;

        private void Start()
        {
            _text = GetComponentInChildren<Text>();
            // _text.text = "<here>";
        }
    }
}