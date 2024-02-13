using Events;
using Manager;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Items
{
    public class ItemKey_1 : ItemBase
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log("ItemKey_1 OnNetworkDespawn");
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

        protected override void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter: " + other.tag);
            if (Interactable.Value && other.CompareTag("Player"))
            {
                Debug.Log("OnTriggerEnter: " + other.tag);
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer && (itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility))
                {
                    Debug.Log("OnTriggerEnter: " + other.tag);
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

        private void OnInteract()
        {
            Debug.Log("OnInteract: " + GameManager.Instance.LocalPlayer.playerType);
            OnInteractServerRpc(GameManager.Instance.LocalPlayer.playerType);
            Destroy(gameObject);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnInteractServerRpc(ItemAccessbility playerType)
        {
            if(!itemDataItem.interactable)
            {
                return;
            }
            SetInteractableServerRpc(false);
            GiveKeyToClientRpc(playerType);
        }

        [ClientRpc]
        private void GiveKeyToClientRpc(ItemAccessbility playerType)
        {
            Debug.Log("GiveKeyToClientRpc: " + playerType);
            new KeyCollectEvent(itemDataItem.item_sub_id, playerType);
        }
    }
}