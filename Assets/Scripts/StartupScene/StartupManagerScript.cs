using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupManagerScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Startup());
        //GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(1);
    }

    IEnumerator Startup()
    {
        var wait = new WaitForSeconds(0.5f);
        var learn = GameObject.Find("Learn");
        var to = GameObject.Find("To");
        var code = GameObject.Find("Code");

        learn.GetComponent<Animator>().SetTrigger("Show");
        yield return wait;
        to.GetComponent<Animator>().SetTrigger("Show");
        yield return wait;
        code.GetComponent<Animator>().SetTrigger("Show");
        yield return wait;
        yield return wait;
        learn.GetComponent<Animator>().SetTrigger("Hide");
        //yield return wait;
        to.GetComponent<Animator>().SetTrigger("Hide");
        //yield return wait;
        code.GetComponent<Animator>().SetTrigger("Hide");
        yield return wait;

        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
