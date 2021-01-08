using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManagerScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        List<int> unlockedLevels = new List<int>() { 1, 2, 3, 4, 5,6,7 };
        StartCoroutine("ExtendPipes", unlockedLevels);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ExtendPipes(List<int> unlockedLevels)
    {
        var pipeBasket = GameObject.Find("Pipes").transform;

        for(int i=0; i< pipeBasket.childCount; i++)
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
