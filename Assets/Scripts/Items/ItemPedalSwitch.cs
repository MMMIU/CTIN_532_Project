using Events;
using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Items
{
    public class ItemPedalSwitch : ItemBase
    {
        [SerializeField]
        GameObject emissionPlne;

        [SerializeField]
        ParticleSystem particles;

        [SerializeField]
        bool accessableByAll = false;

        bool isActive = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("ItemPedalSwitch OnNetworkSpawn");
            emissionPlne.SetActive(false);
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEventServer);
            base.OnNetworkDespawn();
        }

        private void OnTriggerStay(Collider other)
        {
            if (isActive)
            {
                return;
            }

            if (Interactable.Value)
            {
                bool allowTrigger = accessableByAll;
                if (!allowTrigger && other.TryGetComponent<Player>(out var p) && p.IsLocalPlayer)
                {
                    allowTrigger = itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility;
                }

                if (allowTrigger)
                {
                    isActive = true;
                    emissionPlne.SetActive(true);
                    particles.Play();
                    if (TryGetComponent<QuestProgressModifier>(out var qpm))
                    {
                        qpm.AddProgress();
                    }
                    if (TryGetComponent<ItemInteractableModifier>(out var imf))
                    {
                        imf.SetInteractable(true);
                    }
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (Interactable.Value)
            {
                bool allowTrigger = accessableByAll;
                if (!allowTrigger && other.TryGetComponent<Player>(out var p) && p.IsLocalPlayer)
                {
                    allowTrigger = itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility;
                }

                if (allowTrigger)
                {
                    isActive = false;
                    emissionPlne.SetActive(false);
                    particles.Stop();
                    if (TryGetComponent<QuestProgressModifier>(out var qpm))
                    {
                        qpm.DecreaseProgress();
                    }
                    if (TryGetComponent<ItemInteractableModifier>(out var imf))
                    {
                        imf.SetInteractable(false);
                    }
                }
            }

        }

        private void DoItemSetInteractableEventServer(EventBase baseEvent)
        {
            if (!IsServer) return;
            ItemSetInteractableEvent e = baseEvent as ItemSetInteractableEvent;
            if (e.item_uid == item_uid)
            {
                Debug.Log("Item " + item_uid + "DoItemSetInteractableEventServer: " + e.interactable);
                SetInteractableServerRpc(e.interactable);
            }
        }
    }
}