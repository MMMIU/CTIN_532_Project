using Events;
using Manager;
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
    public class ItemMazeGate : ItemBase
    {
        [SerializeField]
        //private Rigidbody rb;
        private GameObject door;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("Item Maze Gate OnNetworkSpawn");
            // freeze rb Y rotation
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        protected override void DoItemSetInteractableEvent(EventBase baseEvent)
        {
            base.DoItemSetInteractableEvent(baseEvent);
            if (baseEvent is ItemSetInteractableEvent e && e.item_uid == item_uid)
            {
                Debug.Log("ItemMazeGate DoItemSetInteractableEvent " + e.interactable);
                //rb.constraints = e.interactable ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotation;
                door.SetActive(!e.interactable);
            }
        }
    }
}
