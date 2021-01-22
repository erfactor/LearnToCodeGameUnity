using Profiles;
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
        public TextAsset hint;

        public Animator animator;

        private bool _isUnlocked = false;

        public bool IsUnlocked
        {
            get
            {
                return _isUnlocked;
            }
            set
            {
                _isUnlocked = value;
                if (value)
                {
                    transform.Find("Padlock").gameObject.SetActive(false);
                }
            }
        }

        public void SpeedUpCog()
        {
            if (_isUnlocked)
            {
                transform.Find("Cog").GetComponent<CogRotateScript>().SpeedUp();
            }
        }

        public void SlowDownCog()
        {
            transform.Find("Cog").GetComponent<CogRotateScript>().SlowDown();
        }

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

            if (GameObject.Find("ProfileManager").GetComponent<ProfileManager>().ShouldPlayTutorial())
            {
                PlayTutorial();
            }
            else
            {
                PlayNormalLevel();
            }
                     
        }

        public void SetButtonColor(Color color)
        {
            //var newColors = GetComponentInChildren<Button>().colors;
            //newColors.normalColor = color;
            //GetComponentInChildren<Button>().colors = newColors;
            GetComponentInChildren<Image>().color = color;
        }

        public void PlayTutorial()
        {
            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(4);
        }

        public void PlayNormalLevel()
        {
            LevelLoader.Level = new Level
            {
                Number = levelIndex,
                Hint = hint.text,
                File = level.text,
                SolutionFile = levelSolution.text
            };

            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(3);
        }


    }
}