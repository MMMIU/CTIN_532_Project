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
    public class ItemFence : ItemBase
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("ItemFence OnNetworkSpawn");
            Interactable.OnValueChanged += OnInteractableChanged;
        }

        public override void OnNetworkDespawn()
        {
            Interactable.OnValueChanged -= OnInteractableChanged;
            base.OnNetworkDespawn();
        }

        private void OnInteractableChanged(bool oldValue, bool newValue)
        {
            Debug.Log("ItemFence OnInteractableChanged");
            if (newValue)
            {

            }
            else
            {
            }
        }
    }
}
