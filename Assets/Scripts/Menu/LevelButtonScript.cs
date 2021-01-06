using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class LevelButtonScript : MonoBehaviour
    {
        private Button button;

        public LevelLoader LevelLoaderPrefab;

        public TextAsset levelData;

        // Start is called before the first frame update
        private void Start()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(LoadLevel);
        }

        private void LoadLevel()
        {
            var loader = Instantiate<LevelLoader>(LevelLoaderPrefab);
            loader.gameObject.name = "LevelLoader";
            loader.LoadLevel(levelData.text);

            DontDestroyOnLoad(loader.gameObject);

            SceneManager.LoadScene(1);
            var animator = GameObject.Find("Canvas").GetComponent<Animator>();
            animator.SetTrigger("ChangeScene");

            
        }
    }
}