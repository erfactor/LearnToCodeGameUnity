using UnityEngine;
using UnityEngine.UI;

namespace Animators
{
    public class PieceText : MonoBehaviour
    {
        public string Text
        {
            set => GetComponentInChildren<Text>().text = value; 
            get => GetComponentInChildren<Text>().text; 
        }
    }
}