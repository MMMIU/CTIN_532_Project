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
    private List<SFXPair> sfxList;

    [SerializeField]
    private AudioSource musicAudioSource;

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
        if (sfxList.Exists(sfx => sfx.name == name))
        {
            musicAudioSource.clip = sfxList.Find(sfx => sfx.name == name).file;
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

    public void PlayOneShot(string name)
    {
        if (musicAudioSource == null)
        {
            Debug.LogWarning("No audio source set for SFXManager");
            return;
        }
        if (sfxList.Exists(sfx => sfx.name == name))
        {
            musicAudioSource.PlayOneShot(sfxList.Find(sfx => sfx.name == name).file);
        }
        else
        {
            Debug.LogWarning("No SFX found with name: " + name);
        }
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