using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManagerScript : MonoBehaviour
{
    public AudioClip unlockedLevelClick;
    public AudioClip lockedLevelClick;
    public AudioClip returnClip;
    public AudioClip sceneChangeClip;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void PlayChangeSceneSound()
    {
        audioSource.clip = sceneChangeClip;
        audioSource.Play();
    }

    public void PlayLockedLevelSound()
    {
        audioSource.clip = lockedLevelClick;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
