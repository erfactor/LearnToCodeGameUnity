using System.Collections.Generic;
using UnityEngine;

public class SFXManagerScript : MonoBehaviour
{
    public AudioClip unlockedLevelClick;
    public AudioClip lockedLevelClick;
    public AudioClip returnClip;
    public AudioClip sceneChangeClip;

    public AudioClip instructionGrab;
    public AudioClip instructionDrop;
    public AudioClip instructionDelete;

    public AudioClip levelCompleted;

    public AudioClip changeSolution;

    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource audioSource4;

    private List<AudioSource> audioSources = new List<AudioSource>();
    public bool SoundMuted { get; private set; } = true;

    private void Start()
    {
        audioSources = new List<AudioSource> {audioSource1, audioSource2, audioSource3, audioSource4};
        UnmuteSound();
        DontDestroyOnLoad(this);
    }

    private AudioSource GetFreeAudioSource()
    {
        foreach (var v in audioSources)
            if (v.isPlaying == false)
                return v;

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

    public void PlayInstructionDeleteSound()
    {
        InternalPlaySound(instructionDelete);
    }

    public void PlaySolutionChangeSound()
    {
        InternalPlaySound(changeSolution);
    }

    public void MuteSound()
    {
        SoundMuted = true;
        foreach (var v in audioSources) v.volume = 0.0f;
    }

    public void UnmuteSound()
    {
        SoundMuted = false;
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