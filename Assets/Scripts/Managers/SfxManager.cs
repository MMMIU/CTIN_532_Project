using Events;
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

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<TaskCompleteEvent>(OnTaskComplete);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<TaskCompleteEvent>(OnTaskComplete);
    }

    private void OnTaskComplete(TaskCompleteEvent e)
    {
        if (e.taskDataItem.task_chain_id == 1 && e.taskDataItem.task_sub_id == 2)
        {
            PlaySFX("level_success");
        }
        else if (e.taskDataItem.task_chain_id == 3 && e.taskDataItem.task_sub_id == 2)
        {
            PlaySFX("level_success");
        }
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