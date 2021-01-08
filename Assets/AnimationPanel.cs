using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationPanel : MonoBehaviour
{
    public Animator Animator;
    public static int LoadedSceneIndex = 2;
    void Start()
    {
        DontDestroyOnLoad(this);
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

    public void LoadNewSceneTest()
    {
        SceneManager.LoadScene(LoadedSceneIndex);
        Animator.SetTrigger("Uncover");
    }
}
