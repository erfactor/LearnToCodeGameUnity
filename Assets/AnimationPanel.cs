using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationPanel : MonoBehaviour
{
    public Animator Animator;
    private int LoadedSceneIndex;
    void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
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
