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

    public AudioClip confetti;

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
        audioSourceMusic.loop = true;
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

    public void PlayConfettiSound()
    {
        InternalPlaySound(confetti);
    }

    public void PlayLevelCompletedSound()
    {
        StartCoroutine(CoroutineLevelCompletedSound());
    }

    private IEnumerator CoroutineLevelCompletedSound()
    {
        yield return CoroutineLowerMusicVolumeOverTime(10);
        InternalPlaySound(levelCompleted);
        yield return new WaitForSeconds(2.0f);
        yield return CoroutineRaiseMusicVolumeOverTime(10);
    }

    public void MuteSound()
    {
        StopAllSounds();
        SoundMuted = true;
        foreach (var v in audioSources) v.volume = 0.0f;
        LowerMusicVolumeOverTime();
    }

    public void UnmuteSound()
    {
        SoundMuted = false;
        foreach (var v in audioSources) v.volume = 1.0f;
        RaiseMusicVolumeOverTime();
    }

    public void LowerMusicVolumeOverTime()
    {
        StopAllCoroutines();
        StartCoroutine(CoroutineLowerMusicVolumeOverTime());
    }

    IEnumerator CoroutineLowerMusicVolumeOverTime(int step = 5)
    {
        for(int i=100; i>=0; i -= step)
        {
            float newVolume = i / 100.0f;
            if (newVolume > audioSourceMusic.volume) continue;
            audioSourceMusic.volume = newVolume;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public void RaiseMusicVolumeOverTime()
    {
        StopAllCoroutines();
        StartCoroutine(CoroutineRaiseMusicVolumeOverTime());
    }

    IEnumerator CoroutineRaiseMusicVolumeOverTime(int step = 5)
    {
        if (SoundMuted) yield break;
        for (int i = 0; i <= 100; i += step)
        {
            float newVolume = i / 100.0f;
            if (newVolume < audioSourceMusic.volume) continue;
            audioSourceMusic.volume = newVolume;
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

    private void StopAllSounds()
    {
        foreach (var v in audioSources) v.Stop();
    }
}