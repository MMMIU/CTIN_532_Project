using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using Inputs;
using Events;

namespace Items
{
    public abstract class ItemBase : NetworkBehaviour
    {
        public int item_uid;
        public ItemDataItem itemDataItem;

        [SerializeField]
        protected InputReader inputReader;

        [SerializeField]
        private NetworkVariable<bool> interactable = new(false);
        public NetworkVariable<bool> Interactable
        {
            get => interactable;
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SetInteractableServerRpc(bool interactable)
        {
            this.interactable.Value = interactable;
            this.interactable.SetDirty(true);
        }


        public override void OnNetworkSpawn()
        {
            itemDataItem = ItemLogic.Instance.GetItemData(item_uid);
            EventManager.Instance.Subscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEvent);
            SetInteractableServerRpc(itemDataItem.init_interactable);
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe(nameof(ItemSetInteractableEvent), DoItemSetInteractableEvent);
        }

        protected virtual void DoItemSetInteractableEvent(EventBase baseEvent)
        {
            if(baseEvent is ItemSetInteractableEvent e && e.item_uid == item_uid)
            {
                SetInteractableServerRpc(e.interactable);
            }
        }
    }
}
