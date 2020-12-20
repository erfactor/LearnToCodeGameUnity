using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class LevelButtonScript : MonoBehaviour
    {
        public string filename;

        private Button button;

        // Start is called before the first frame update
        private void Start()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(LoadLevel);
        }

        private void LoadLevel()
        {
            Debug.Log($"Loading level with name: [{filename}]");
            LevelLoader.LevelFileName = filename;
            var animator = GameObject.Find("Canvas").GetComponent<Animator>();
            animator.SetTrigger("ChangeScene");
        }
    }
}