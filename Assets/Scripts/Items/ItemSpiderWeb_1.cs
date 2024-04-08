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
            base.OnNetworkSpawn();
            Debug.Log("ItemSpiderWeb_1 OnNetworkSpawn");
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        [ClientRpc]
        protected override void SetInteractableClientRpc(bool interactable)
        {
            if(interactable)
            {
                // set material DBlendFactor to 1
                Material material = GetComponent<MeshRenderer>().material;
                material.SetFloat("_DBlendFactor", 1);
            }
        }
    }
}
