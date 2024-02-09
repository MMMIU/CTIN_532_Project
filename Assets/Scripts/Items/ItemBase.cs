using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using Inputs;

namespace Items
{
    public abstract class ItemBase : NetworkBehaviour
    {
        public int item_uid;
        public ItemDataItem itemDataItem;

        [SerializeField]
        protected InputReader inputReader;

        private NetworkVariable<bool> interactable = new(false);
        public NetworkVariable<bool> Interactable
        {
            get => interactable;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetInteractableServerRpc(bool interactable)
        {
            this.interactable.Value = interactable;
        }


        protected virtual void Awake()
        {
            itemDataItem = ItemLogic.Instance.GetItemData(item_uid);
        }

        protected virtual void OnTriggerEnter(Collider other) { }
        protected virtual void OnTriggerExit(Collider other) { }


    }
}
