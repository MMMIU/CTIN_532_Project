using Events;
using Managers;
using Players;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory main;
    // Start is called before the first frame update
    bool key1 = false;
    public bool HasKey1 { get => key1; }

    bool key2 = false;
    public bool HasKey2 { get => key2; }
    void Awake()
    {
        main = this;
        EventManager.Instance.Subscribe<KeyCollectEvent>(CollectKey);
    }

    private void CollectKey(KeyCollectEvent baseEvent)
    {
        if (baseEvent.playerType != GameManager.Instance.LocalPlayer.playerType)
        {
            return;
        }
        if(baseEvent.keyIndex == 1)
        {
            key1 = true;
        }
        else if(baseEvent.keyIndex == 2)
        {
            key2 = true;
        }
        EventManager.Instance.Unsubscribe<KeyCollectEvent>(CollectKey);
    }

}
