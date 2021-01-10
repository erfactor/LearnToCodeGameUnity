using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManagerScript : MonoBehaviour
{
    public AudioClip unlockedLevelClick;
    public AudioClip lockedLevelClick;
    public AudioClip returnClip;
    public AudioClip sceneChangeClip;

    public AudioClip instructionGrab;
    public AudioClip instructionDrop;

    public AudioClip levelCompleted;

    public AudioClip changeSolution;

    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource audioSource4;

    List<AudioSource> audioSources = new List<AudioSource>();

    // Start is called before the first frame update
    void Start()
    {
        audioSources = new List<AudioSource>() { audioSource1, audioSource2, audioSource3, audioSource4 };
        UnmuteSound();
        DontDestroyOnLoad(this);
    }

    private AudioSource GetFreeAudioSource()
    {
        foreach(var v in audioSources)
        {
            if (v.isPlaying == false) return v;
        }

        return null;
    }

    public void PlayChangeSceneSound()
    {
        InternalPlaySound(sceneChangeClip);
    }

    public void PlayLockedLevelSound()
    {
        InternalPlaySound(lockedLevelClick);
    }

    public void PlayInstructionGrabSound()
    {
        InternalPlaySound(instructionGrab);
    }

    public void PlayInstructionDropSound()
    {
        InternalPlaySound(instructionDrop);
    }

    public void PlaySolutionChangeSound()
    {
        InternalPlaySound(changeSolution);
    }

    public void MuteSound()
    {
        foreach(var v in audioSources) v.volume = 0.0f;
    }

    public void UnmuteSound()
    {
        foreach (var v in audioSources) v.volume = 1.0f;
    }

    private void InternalPlaySound(AudioClip audioClip)
    {
        var audioSource = GetFreeAudioSource();
        if (audioSource == null) return;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

}
