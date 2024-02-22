using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    [System.Serializable]
    private class SFXPair
    {
        public string name;
        public AudioClip file;
    }

    [SerializeField]
    private List<SFXPair> audioClipList;

    [SerializeField]
    private AudioSource musicAudioSource;

    [SerializeField]
    private AudioSource soundAudioSource;

    [SerializeField]
    private AudioMixer mixer;

    private static SFXManager instance;
    public static SFXManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayMusic(string name)
    {
        if (musicAudioSource == null)
        {
            Debug.LogWarning("No audio source set for SFXManager");
            return;
        }
        if (audioClipList.Exists(sfx => sfx.name == name))
        {
            musicAudioSource.clip = audioClipList.Find(sfx => sfx.name == name).file;
            musicAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No SFX found with name: " + name);
        }
    }

    public void StopMusic()
    {
        if (musicAudioSource == null)
        {
            Debug.LogWarning("No audio source set for SFXManager");
            return;
        }
        musicAudioSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (soundAudioSource == null)
        {
            Debug.LogWarning("No audio source set for SFXManager");
            return;
        }
        if (audioClipList.Exists(sfx => sfx.name == name))
        {
            soundAudioSource.clip = audioClipList.Find(sfx => sfx.name == name).file;
            soundAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No SFX found with name: " + name);
        }
    }

    public void StopSFX()
    {
        if (soundAudioSource == null)
        {
            Debug.LogWarning("No audio source set for SFXManager");
            return;
        }
        soundAudioSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        if(mixer==null)
        {
            Debug.LogWarning("No audio mixer set for SFXManager");
            return;
        }
        mixer.SetFloat("Music", volume);
    }

    public void SetSoundVolume(float volume)
    {
        if (mixer == null)
        {
            Debug.LogWarning("No audio mixer set for SFXManager");
            return;
        }
        mixer.SetFloat("Sound", volume);
    }

    public void SetMasterVolume(float volume)
    {
        if (mixer == null)
        {
            Debug.LogWarning("No audio mixer set for SFXManager");
            return;
        }
        mixer.SetFloat("Master", volume);
    }
}