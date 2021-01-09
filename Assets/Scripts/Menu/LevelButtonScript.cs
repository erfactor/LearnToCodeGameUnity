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

        public TextAsset level;
        public TextAsset levelSolution;

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
            var loader = Instantiate(LevelLoaderPrefab);
            loader.gameObject.name = "LevelLoader";
            loader.LoadSolution(levelSolution.text);
            loader.LoadLevel(level.text);
            DontDestroyOnLoad(loader.gameObject);

            var animator = GameObject.Find("AnimationPanel").GetComponent<Animator>();
            animator.SetTrigger("Cover");            
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