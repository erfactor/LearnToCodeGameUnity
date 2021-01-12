using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupManagerScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Startup());
    }

    IEnumerator Startup()
    {
        if (Config.Debug.SkipIntroAnimation)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(1);
        }

        var showInterval = new WaitForSeconds(Config.Timing.StartupWordsShowInterval);
        var hideInterval = new WaitForSeconds(Config.Timing.StartupWordsHideInterval);
        var stayDuration = new WaitForSeconds(Config.Timing.StartupWordsStayDuration);
        var afterHideDuration = new WaitForSeconds(Config.Timing.StartupAfterHideDuration);

        var learn = GameObject.Find("Learn");
        var to = GameObject.Find("To");
        var code = GameObject.Find("Code");

        learn.GetComponent<Animator>().SetTrigger("Show");
        yield return showInterval;
        to.GetComponent<Animator>().SetTrigger("Show");
        yield return showInterval;
        code.GetComponent<Animator>().SetTrigger("Show");
        yield return showInterval;

        yield return stayDuration;

        code.GetComponent<Animator>().SetTrigger("Hide");
        yield return hideInterval;
        to.GetComponent<Animator>().SetTrigger("Hide");
        yield return hideInterval;
        learn.GetComponent<Animator>().SetTrigger("Hide");
        yield return hideInterval;

        yield return afterHideDuration;

        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
