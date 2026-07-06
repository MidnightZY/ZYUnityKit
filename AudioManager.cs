using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private readonly Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();


    protected override void Awake()
    {
        base.Awake();

        EnsureAudioSources();
    }

    public static void PlayMusic(AudioClip clip, bool loop = true)
    {
        Instance.PlayMusicInternal(clip, loop);
    }

    public static void PlayMusic(string clipPath, bool loop = true)
    {
        Instance.PlayMusicInternal(clipPath, loop);
    }

    public static void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        Instance.PlaySfxInternal(clip, volumeScale);
    }

    public static void PlaySfx(string clipPath, float volumeScale = 1f)
    {
        Instance.PlaySfxInternal(clipPath, volumeScale);
    }

    public static void StopMusic()
    {
        Instance.musicSource.Stop();
    }

    public static void PauseMusic()
    {
        Instance.musicSource.Pause();
    }

    public static void ResumeMusic()
    {
        Instance.musicSource.UnPause();
    }

    public static bool IsMusicPlaying()
    {
        return Instance.musicSource.isPlaying;
    }

    public static void SetMusicVolume(float volume)
    {
        Instance.musicSource.volume = Mathf.Clamp01(volume);
    }

    public static void SetSfxVolume(float volume)
    {
        Instance.sfxSource.volume = Mathf.Clamp01(volume);
    }

    public static void SetAllVolume(float volume)
    {
        SetMusicVolume(volume);
        SetSfxVolume(volume);
    }

    private void PlayMusicInternal(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("Music clip is null");
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    private void PlayMusicInternal(string clipPath, bool loop = true)
    {
        AudioClip clip = LoadClip(clipPath);
        PlayMusicInternal(clip, loop);
    }

    private void PlaySfxInternal(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SFX clip is null");
            return;
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    private void PlaySfxInternal(string clipPath, float volumeScale = 1f)
    {
        AudioClip clip = LoadClip(clipPath);
        PlaySfxInternal(clip, volumeScale);
    }

    private AudioClip LoadClip(string clipPath)
    {
        if (string.IsNullOrWhiteSpace(clipPath))
        {
            Debug.LogWarning("Audio clip path is empty");
            return null;
        }

        if (clipCache.TryGetValue(clipPath, out AudioClip cachedClip))
        {
            return cachedClip;
        }

        AudioClip clip = Resources.Load<AudioClip>(clipPath);

        if (clip == null)
        {
            Debug.LogWarning($"Audio clip not found in Resources: {clipPath}");
            return null;
        }

        clipCache.Add(clipPath, clip);
        return clip;
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }
}
