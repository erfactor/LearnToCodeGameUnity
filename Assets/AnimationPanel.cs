using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationPanel : MonoBehaviour
{
    public Animator Animator;
    private int LoadedSceneIndex;
    void Start()
    {
        DontDestroyOnLoad(this.transform.parent.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Animator.SetTrigger("Cover");
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Animator.SetTrigger("Uncover");
        }
    }

    public void ChangeScene(int newSceneIndex)
    {
        GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayChangeSceneSound();
        LoadedSceneIndex = newSceneIndex;
        Animator.SetTrigger("Cover");
    }

    public void LoadNewSceneTest()
    {
        SceneManager.LoadScene(LoadedSceneIndex);
        Animator.SetTrigger("Uncover");
    }
}
