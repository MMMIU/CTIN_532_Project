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

        public override void OnNetworkSpawn()
        {
            Debug.Log("ItemPedalSwitch OnNetworkSpawn");
            SetInteractableServerRpc(itemDataItem.interactable);
            EventManager.Instance.Subscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEventServer);
            emissionPlne.SetActive(false);
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEventServer);
            base.OnNetworkDespawn();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (Interactable.Value && other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.playerType == itemDataItem.accessbility)
                {
                    emissionPlne.SetActive(true);
                    particles.Play();
                    if (p.IsLocalPlayer)
                    {
                        QuestManager.Instance.AddProgressServerRpc(1, 2, 1);
                    }
                }
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (Interactable.Value && other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.playerType == itemDataItem.accessbility)
                {
                    emissionPlne.SetActive(false);
                    particles.Stop();
                    if (p.IsLocalPlayer)
                    {
                        QuestManager.Instance.DecreaseProgressServerRpc(1, 2, 1);
                    }
                }
            }
        }

        private void DoItemSetInteractableEventServer(BaseEvent baseEvent)
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