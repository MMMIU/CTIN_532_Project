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
using Unity.Services.Relay;
using UnityEngine;


namespace Items
{
    public class ItemMazeEye : ItemBase
    {
        [SerializeField]
        private float cooldown = 10f;

        float timer = -1f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("ItemMazeEye OnNetworkSpawn");
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        private void Update()
        {
            if (timer < 0)
            {
                return;
            }
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                timer = -1f;
                SetInteractableServerRpc(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer && (itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility))
                {
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
            if (timer >= 0f)
            {
                UIManager.Instance.OpenPanel<UIPopUpBar>().SetPopUpText("Still on cooldown, " + (int)(cooldown - timer) + " seconds remaining");
                return;
            }
            Debug.Log("ItemMazeEye OnInteract" + transform.position);
            UIManager.Instance.OpenPanel<UIOverviewPanel>(transform.position);
            timer = 0f;
            SetInteractableServerRpc(false);
        }
    }
}
