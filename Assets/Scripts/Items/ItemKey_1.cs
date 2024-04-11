using Events;
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
            base.OnNetworkSpawn();
            Debug.Log("ItemKey_1 OnNetworkDespawn");
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter: " + other.tag);
            if (Interactable.Value && other.CompareTag("Player"))
            {
                Debug.Log("OnTriggerEnter: " + other.tag);
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer)
                {
                    Debug.Log("OnTriggerEnter: " + other.tag);
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
            Debug.Log("OnInteract: " + GameManager.Instance.LocalPlayer.playerType);
            OnInteractServerRpc(GameManager.Instance.LocalPlayer.playerType);
            gameObject.SetActive(false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnInteractServerRpc(ItemAccessbility playerType)
        {
            SetInteractableServerRpc(false);
            GiveKeyToClientRpc(playerType);
        }

        [ClientRpc]
        private void GiveKeyToClientRpc(ItemAccessbility playerType)
        {
            Debug.Log("GiveKeyToClientRpc: " + playerType);
            new KeyCollectEvent(itemDataItem.item_sub_id, playerType);
            gameObject.SetActive(false);
            SFXManager.Instance.PlaySFX("interaction");
            inputReader.InteractionEvent -= OnInteract;
        }
    }
}