using Events;
using Managers;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource inBattleBGM;
    public AudioSource outsideBattleBGM;
    public int inBattleBGMRequest = 0;
    public static BackgroundMusicManager instance;
    void Awake()
    {
        instance = this;
        EventManager.Instance.Subscribe<EnemyChaseStart>(BattleMusicRequestHandle);
        EventManager.Instance.Subscribe<EnemyChaseEnd>(OutsideBattleMusicRequestHandle);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe<EnemyChaseStart>(BattleMusicRequestHandle);
        EventManager.Instance.Unsubscribe<EnemyChaseEnd>(OutsideBattleMusicRequestHandle);
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
        if (!inBattleBGM.isPlaying)
        {
            //Debug.Log("switched to inBattleBGM");
            outsideBattleBGM.Pause();
            inBattleBGM.Play();
        }
    }
    public void SwitchToOutsideBattleBGM()
    {
        //Debug.Log("SwitchToOutsideBattleBGM is called");
        if (!outsideBattleBGM.isPlaying)
        {
            //Debug.Log("switched to outsideBattleBGM");
            inBattleBGM.Pause();
            outsideBattleBGM.Play();
        }
    }
}
