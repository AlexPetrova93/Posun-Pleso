using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioRef
{
    Music,
    MoneyRulez,
    GameOver,
    PipePlaceTurn,
    GlassCrack,
    Bubbles,
    Mumbling,
    Fixing,
    Win
}

[Serializable]
public struct AudioItem
{
    public AudioRef AudioRef;
    public AudioClip AudioClip;
}

[Serializable]
public struct AudioItemWithVariations
{
    public AudioRef AudioRef;
    public AudioClip[] AudioClips;
    public int LastIdx;
}

public struct BufferedAudioSource
{
    public AudioSource[] Sources;
    public int LastIdx;
}

public class SoundManager : MonoBehaviour
{
    public AudioItem[] MusicItems;
    public AudioItem[] SfxItems;
    public AudioItemWithVariations[] SfxVariationItems;

    private Dictionary<AudioRef, AudioSource> _musicClips = new();
    private Dictionary<AudioRef, BufferedAudioSource> _sfxClips = new();
    private Dictionary<AudioRef, BufferedAudioSource> _sfxVariationClips = new();

    public void Init()
    {
        foreach (var musicItem in MusicItems)
        {
            _musicClips.Add(musicItem.AudioRef, CreateAudioSource(musicItem, true));
        }
        foreach (var sfxItem in SfxItems)
        {
            _sfxClips.Add(sfxItem.AudioRef, CreateBufferedAudioSource(sfxItem));
        }
        foreach (var sfxItem in SfxVariationItems)
        {
            _sfxVariationClips.Add(sfxItem.AudioRef, CreateAudioSources(sfxItem));
        }
    }

    private BufferedAudioSource CreateAudioSources(AudioItemWithVariations audioItem)
    {
        var ret = new BufferedAudioSource();
        ret.Sources = new AudioSource[audioItem.AudioClips.Length];
        var go = new GameObject(audioItem.AudioRef.ToString());
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        for (var i = 0; i < audioItem.AudioClips.Length; i++)
        {
            var audioClip = audioItem.AudioClips[i];
            var src = go.AddComponent<AudioSource>();
            src.clip = audioClip;
            src.playOnAwake = false;
            ret.Sources[i] = src;
        }

        return ret;
    }

    private AudioSource CreateAudioSource(AudioItem audioItem, bool loop)
    {
        var go = new GameObject(audioItem.AudioRef.ToString());
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        var src = go.AddComponent<AudioSource>();
        src.clip = audioItem.AudioClip;
        src.loop = loop;
        src.playOnAwake = false;
        return src;
    }
    
    private BufferedAudioSource CreateBufferedAudioSource(AudioItem audioItem, int bufferLen = 2)
    {
        var bufSrc = new BufferedAudioSource();
        bufSrc.Sources = new AudioSource[bufferLen];
        for (var i = 0; i < bufSrc.Sources.Length; i++)
        {
            bufSrc.Sources[i] = CreateAudioSource(audioItem, false);
        }
        
        return bufSrc;
    }

    public void PlayMusic(AudioRef audioRef)
    {
        var src = _musicClips[audioRef];
        src.Play();
    }

    public void PlayMusicAtVolume(AudioRef audioRef, float volume)
    {
        var src = _musicClips[audioRef];
        src.volume = volume;
        src.Play();
    }
    
    public void PauseMusic(AudioRef audioRef)
    {
        var src = _musicClips[audioRef];
        src.Pause();
    }

    public void PlaySound(AudioRef audioRef)
    {
        var src = _sfxClips.ContainsKey(audioRef) ? _sfxClips[audioRef] : _sfxVariationClips[audioRef];
        var idx = src.LastIdx;
        src.Sources[idx].Play();
        ++idx;
        idx %= src.Sources.Length;
        src.LastIdx = idx;
        if (_sfxClips.ContainsKey(audioRef)) _sfxClips[audioRef] = src;
        else _sfxVariationClips[audioRef] = src;
    }
}
