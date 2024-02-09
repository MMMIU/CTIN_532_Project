using Events;
using Managers;
using Players;
using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;


namespace Items
{
    public class ItemBookOne : ItemBase
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log("ItemBookOne OnNetworkSpawn");
            SetInteractableServerRpc(itemDataItem.interactable);
            EventManager.Instance.Subscribe("ItemSetInteractableEvent", DoItemSetInteractableEventServer);
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe("ItemSetInteractableEvent", DoItemSetInteractableEventServer);
            base.OnNetworkDespawn();
        }

        protected override void OnTriggerEnter(Collider other)
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

        protected override void OnTriggerExit(Collider other)
        {
            if (Interactable.Value && other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer && (itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility))
                {
                    inputReader.InteractionEvent -= OnInteract;
                }
            }
        }

        private void DoItemSetInteractableEventServer(BaseEvent baseEvent)
        {
            if (!IsServer) return;
            ItemSetInteractableEvent e = baseEvent as ItemSetInteractableEvent;
            Debug.Log("Try Set: " + e.item_uid + "::DoItemSetInteractableEventServer: " + e.interactable);
            if (e.item_uid == item_uid)
            {
                Debug.Log("Item " + item_uid + "::DoItemSetInteractableEventServer: " + e.interactable);
                SetInteractableServerRpc(e.interactable);
            }
        }

        private void OnInteract()
        {
            QuestManager.Instance.AddProgressServerRpc(1, 1, 1);
            SetInteractableServerRpc(false);
        }
    }
}
