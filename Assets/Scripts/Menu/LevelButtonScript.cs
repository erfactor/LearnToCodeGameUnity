using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class LevelButtonScript : MonoBehaviour
    {
        public int levelIndex;

        private Button button;

        public LevelLoader LevelLoaderPrefab;

        public TextAsset levelData;

        // Start is called before the first frame update
        private void Start()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(LoadLevel);
            UpdateText();
        }

        public void UpdateText()
        {
            GetComponentInChildren<Text>().text = levelIndex.ToString();
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

        public void SetButtonColor(Color color)
        {
            //var newColors = GetComponentInChildren<Button>().colors;
            //newColors.normalColor = color;
            //GetComponentInChildren<Button>().colors = newColors;
            GetComponentInChildren<Image>().color = color;
        }
    }
}