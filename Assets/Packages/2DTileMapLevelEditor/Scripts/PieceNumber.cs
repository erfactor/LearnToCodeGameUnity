using UnityEngine;

namespace Packages._2DTileMapLevelEditor.Scripts
{
    public class PieceNumber : MonoBehaviour
    {
        public void SetPieceNumber(string inputFieldNumber)
        {
            FindObjectOfType<LevelEditor>().PieceNumberText = inputFieldNumber;
        }
    }
}