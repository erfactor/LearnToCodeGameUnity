using Services;
using UnityEngine;
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

        public Animator animator;

        public bool IsUnlocked { get; set; }

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
            if (!IsUnlocked)
            {
                GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayLockedLevelSound();
                animator.SetTrigger("Shake");
                return;
            }

            DestroyImmediate(GameObject.Find("LevelLoader"));
            var loader = Instantiate(LevelLoaderPrefab);
            loader.gameObject.name = "LevelLoader";
            loader.LoadSolution(levelSolution.text);
            loader.LoadLevel(level.text, levelIndex);
            DontDestroyOnLoad(loader.gameObject);

            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(2);         
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