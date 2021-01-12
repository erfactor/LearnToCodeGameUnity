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
        if (Config.Debug.SkipSceneChangeAnimation)
        {
            GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayChangeSceneSound();
            GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayMusicOnCurrentScene(newSceneIndex);
            LoadedSceneIndex = newSceneIndex;
            // Animator.SetTrigger("Cover");
            SceneManager.LoadScene(LoadedSceneIndex);
            
        }
        
        else
        {   
            GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayChangeSceneSound();
            GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().LowerMusicVolumeOverTime();
            LoadedSceneIndex = newSceneIndex;
            Animator.SetTrigger("Cover");
        }
    }

    public void LoadNewSceneTest()
    {
        if (Config.Debug.SkipSceneChangeAnimation)
        {

        }
        else
        {
            GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().RaiseMusicVolumeOverTime();
            SceneManager.LoadScene(LoadedSceneIndex);
            Animator.SetTrigger("Uncover");
            GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayMusicOnCurrentScene(LoadedSceneIndex);
        }

    }
}
