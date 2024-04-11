using Events;
using Managers;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource inBattleBGM;
    public AudioSource outsideBattleBGM;
    public int inBattleBGMRequest = 0;
    public float fadeInandOutTime = 2.5f;
    private float fadeInandOutTimeCounter = 0;
    public static BackgroundMusicManager instance;
    public bool turnOnInBattle = false;
    public float scaleForInBattleBGM = 0.8f;
    public float scaleForOutBattleBGM = 1f;
    void Awake()
    {
        instance = this;
        EventManager.Instance.Subscribe<EnemyChaseStart>(BattleMusicRequestHandle);
        EventManager.Instance.Subscribe<EnemyChaseEnd>(OutsideBattleMusicRequestHandle);
        MusicInitialization();
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe<EnemyChaseStart>(BattleMusicRequestHandle);
        EventManager.Instance.Unsubscribe<EnemyChaseEnd>(OutsideBattleMusicRequestHandle);
    }

    private void MusicInitialization()
    {
        inBattleBGM.playOnAwake = true;
        inBattleBGM.volume = 0f;
        inBattleBGM.loop = true;
        inBattleBGM.Play();
        outsideBattleBGM.playOnAwake = true;
        outsideBattleBGM.volume = 1f * scaleForOutBattleBGM;
        outsideBattleBGM.loop = true;
        outsideBattleBGM.Play();
    }

    private void OutsideBattleMusicRequestHandle(EnemyChaseEnd end)
    {
        inBattleBGMRequest--;
        //SwitchMusicCheck();
    }

    private void BattleMusicRequestHandle(EnemyChaseStart start)
    {
        inBattleBGMRequest++;
        //SwitchMusicCheck();
    }

    private void Update()
    {
        if(fadeInandOutTimeCounter > 0)
        {
            fadeInandOutTimeCounter -= Time.deltaTime;
            float fadeInVolume = 1 * ((fadeInandOutTime - fadeInandOutTimeCounter) / fadeInandOutTime);
            float fadeOutVolume = 1 * (fadeInandOutTimeCounter) / fadeInandOutTime;
            if (turnOnInBattle)
            {
                inBattleBGM.volume = scaleForInBattleBGM * fadeInVolume;
                outsideBattleBGM.volume = scaleForOutBattleBGM * fadeOutVolume;
            }
            else
            {
                inBattleBGM.volume = scaleForInBattleBGM * fadeOutVolume;
                outsideBattleBGM.volume = scaleForOutBattleBGM * fadeInVolume;
            }
        }
    }

    private void SwitchMusicCheck()
    {
        Debug.Log("Current Updated Battle Music Request Check: " + inBattleBGMRequest);
        if(inBattleBGMRequest > 0)
        {
            SwitchToBattleBGM();
        }
        else
        {
            SwitchToOutsideBattleBGM();
        }
    }

    public void GameStartBGMPlay()
    {
        if (!outsideBattleBGM.isPlaying)
        {
            outsideBattleBGM.Play();
        }
    }

    public void SwitchToBattleBGM()
    {
        //Debug.Log("SwitchToBattleBGM is called");

        if (!turnOnInBattle)
        {
            turnOnInBattle = true;
            if (fadeInandOutTimeCounter <= 0)
            {
                fadeInandOutTimeCounter = fadeInandOutTime;
            }
            else
            {
                fadeInandOutTimeCounter = fadeInandOutTime - fadeInandOutTimeCounter;
            }
        }

        //if (!inBattleBGM.isPlaying)
        //{
        //    //Debug.Log("switched to inBattleBGM");
        //    outsideBattleBGM.Pause();
        //    inBattleBGM.Play();
        //}
    }
    public void SwitchToOutsideBattleBGM()
    {
        if (turnOnInBattle)
        {
            turnOnInBattle = false;
            if (fadeInandOutTimeCounter <= 0)
            {
                fadeInandOutTimeCounter = fadeInandOutTime;
            }
            else
            {
                fadeInandOutTimeCounter = fadeInandOutTime - fadeInandOutTimeCounter;
            }
        }

        ////Debug.Log("SwitchToOutsideBattleBGM is called");
        //if (!outsideBattleBGM.isPlaying)
        //{
        //    //Debug.Log("switched to outsideBattleBGM");
        //    inBattleBGM.Pause();
        //    outsideBattleBGM.Play();
        //}
    }
}
