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

        public LevelLoader LevelLoaderPrefab;

        // Start is called before the first frame update
        private void Start()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(LoadLevel);
        }

        private void LoadLevel()
        {
            Debug.Log($"Loading level with name: [{filename}]");
            //LevelLoader.LevelFileName = filename;
            

            var loader = Instantiate<LevelLoader>(LevelLoaderPrefab);
            loader.gameObject.name = "LevelLoader";
            loader.PublicLoadLevel(filename);
            DontDestroyOnLoad(loader.gameObject);

            SceneManager.LoadScene(1);
            var animator = GameObject.Find("LevelCanvas").GetComponent<Animator>();
            animator.SetTrigger("ChangeScene");

            
        }
    }
}