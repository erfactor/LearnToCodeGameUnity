using UnityEngine;
using UnityEngine.UI;

namespace Animators
{
    public class PieceText : MonoBehaviour
    {
        private void Start()
        {
        }
        
        public string Text
        {
            set => GetComponentInChildren<Text>().text = value; 
        }
    }
}