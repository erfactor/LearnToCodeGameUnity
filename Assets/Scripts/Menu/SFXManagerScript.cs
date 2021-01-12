using System.Collections;
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

    public AudioClip startupMusic;
    public AudioClip profileMusic;
    public AudioClip mainMenuMusic;
    public AudioClip levelMusic;

    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource audioSource4;

    public AudioSource audioSourceMusic;

    private List<AudioSource> audioSources = new List<AudioSource>();
    private List<AudioClip> musicInScene = new List<AudioClip>();
    public bool SoundMuted { get; private set; } = true;

    private void Start()
    {
        audioSources = new List<AudioSource> {audioSource1, audioSource2, audioSource3, audioSource4};
        musicInScene = new List<AudioClip> { startupMusic, profileMusic, mainMenuMusic, levelMusic};
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
        audioSourceMusic.volume = 0.0f;
    }

    public void UnmuteSound()
    {
        SoundMuted = false;
        foreach (var v in audioSources) v.volume = 1.0f;
        audioSourceMusic.volume = 1.0f;
    }

    public void LowerMusicVolumeOverTime()
    {
        StartCoroutine(CoroutineLowerMusicVolumeOverTime());
    }

    IEnumerator CoroutineLowerMusicVolumeOverTime()
    {
        for(int i=95; i>=0; i -= 5)
        {
            audioSourceMusic.volume = i / 100.0f;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public void RaiseMusicVolumeOverTime()
    {
        StartCoroutine(CoroutineRaiseMusicVolumeOverTime());
    }

    IEnumerator CoroutineRaiseMusicVolumeOverTime()
    {
        for (int i = 0; i < 100; i += 5)
        {
            audioSourceMusic.volume = i / 100.0f;
            yield return new WaitForFixedUpdate();
        }
    }

    public void PlayMusicOnCurrentScene(int sceneIndex)
    {
        audioSourceMusic.Stop();
        audioSourceMusic.clip = musicInScene[sceneIndex];
        if (audioSourceMusic.clip)
        {
            audioSourceMusic.Play();
        }
        else
        {
            Debug.LogWarning($"No music is attached to scene with index {sceneIndex}");
        }
    }

    private void InternalPlaySound(AudioClip audioClip)
    {
        var audioSource = GetFreeAudioSource();
        if (audioSource == null) return;
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}