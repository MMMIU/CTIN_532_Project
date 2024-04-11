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
using UnityEngine;


namespace Items
{
    public class ItemBookOne : ItemBase
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("ItemBookOne OnNetworkSpawn");
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
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
                if (p.IsLocalPlayer)
                {
                    inputReader.InteractionEvent -= OnInteract;
                }
            }
        }

        private void OnInteract()
        {
            SFXManager.Instance.PlaySFX("interaction");
            UIManager.Instance.OpenPanel<UIBookReadPanel>(itemDataItem.desc);
        }
    }
}
