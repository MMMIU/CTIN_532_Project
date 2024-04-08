using Events;
using Managers;
using Players;
using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;
using Util;


namespace Items
{
    public class ItemHanoiControl : ItemBase
    {
        [SerializeField]
        bool hintShown = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("ItemHanoiControl OnNetworkSpawn");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Interactable.Value && other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer && (itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility))
                {
                    inputReader.InteractionEvent += OnInteract;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer && (itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility))
                {
                    inputReader.InteractionEvent -= OnInteract;
                }
            }
        }

        private void OnInteract()
        {
            if (!hintShown)
            {
                hintShown = true;
                UIManager.Instance.OpenPanel<UIHintMenu>(itemDataItem.desc);
                if(TryGetComponent(out QuestProgressModifier questProgressModifier))
                {
                    questProgressModifier.Assign();
                }
            }
            else
            {
                new HanoiControlStartEvent();
            }
        }
    }
}
