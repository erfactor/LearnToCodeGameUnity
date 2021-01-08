using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Menu
{
    public class MainMenuManagerScript : MonoBehaviour
    {
        public Color completedLevelColor;
        public Color unlockedLevelColor;
        public Color lockedLevelColor;


        // Start is called before the first frame update
        void Start()
        {
            List<int> unlockedLevels = new List<int>() { 1, 2, 3 };
            StartCoroutine("ExtendPipes", unlockedLevels);
            SetLevelButtonColors(unlockedLevels);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public List<LevelButtonScript> GetLevelButtonScripts()
        {
            List<LevelButtonScript> list = new List<LevelButtonScript>();
            var container = GameObject.Find("Cogs");
            for(int i=0; i<container.transform.childCount; i++)
            {
                list.Add(container.transform.GetChild(i).GetComponent<LevelButtonScript>());
            }

            return list;
        }

        public void SetLevelButtonColors(List<int> unlockedLevels)
        {
            var levels = GetLevelButtonScripts();
            foreach(var level in levels)
            {
                level.SetButtonColor(GetButtonColor(unlockedLevels, level.levelIndex));
            }
        }

        public Color GetButtonColor(List<int> unlockedLevels, int levelIndex)
        {
            if (IsLevelCompleted(unlockedLevels, levelIndex)) return completedLevelColor;
            if (IsLevelUnlocked(unlockedLevels, levelIndex)) return unlockedLevelColor;
            return lockedLevelColor;
        }

        public bool IsLevelCompleted(List<int> unlockedLevels, int levelIndex)
        {
            if (unlockedLevels == null) return false;
            if (unlockedLevels.Count == 0) return false;
            return levelIndex < unlockedLevels.Max();
        }

        public bool IsLevelUnlocked(List<int> unlockedLevels, int levelIndex)
        {
            if (unlockedLevels == null) return false;
            if (unlockedLevels.Count == 0) return false;
            return unlockedLevels.Contains(levelIndex);
        }



        public IEnumerator ExtendPipes(List<int> unlockedLevels)
        {
            var pipeBasket = GameObject.Find("Pipes").transform;

            for (int i = 0; i < pipeBasket.childCount; i++)
            {
                var childPipeScript = pipeBasket.GetChild(i).GetComponent<PipeScript>();

                if (unlockedLevels.Contains(childPipeScript.LevelFrom))
                {
                    childPipeScript.EmergeAnimation();
                }
            }

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < pipeBasket.childCount; i++)
            {
                var childPipeScript = pipeBasket.GetChild(i).GetComponent<PipeScript>();

                if (unlockedLevels.Contains(childPipeScript.LevelTo))
                {
                    childPipeScript.ExtendAnimation();
                }
            }
        }
    }
}

