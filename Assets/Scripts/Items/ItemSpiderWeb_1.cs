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
    public class ItemSpiderWeb_1 : ItemBase
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log("ItemSpiderWeb_1 OnNetworkSpawn");
            SetInteractableServerRpc(itemDataItem.interactable);
            EventManager.Instance.Subscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEventServer);
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEventServer);
            base.OnNetworkDespawn();
        }

        private void DoItemSetInteractableEventServer(BaseEvent baseEvent)
        {
            if (!IsServer) return;
            ItemSetInteractableEvent e = baseEvent as ItemSetInteractableEvent;
            Debug.Log("Server: Try Set: " + e.item_uid + "::DoItemSetInteractableEventServer: " + e.interactable);
            if (e.item_uid == item_uid)
            {
                Debug.Log("Server: Item " + item_uid + "::DoItemSetInteractableEventServer: " + e.interactable);
                SetInteractableServerRpc(e.interactable);
            }
        }
    }
}
