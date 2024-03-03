using Items;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Utils
{
    public class TransferOwnership : NetworkBehaviour
    {
        public ItemAccessbility playerType;

        public bool isTransfered = false;

        private void Update()
        {
            //if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().playerType == playerType)
            //{
            //    Debug.Log("Requesting ownership to " + playerType);
            //    ulong clientId = NetworkManager.Singleton.LocalClientId;
            //    RequestOwnershipServerRpc(clientId);
            //}
            if(!IsSpawned || isTransfered || IsServer || NetworkManager.Singleton.LocalClient.PlayerObject == null)
            {
                return;
            }
            if (GameManager.Instance.LocalPlayer.playerType == playerType)
            {
                Debug.Log("Requesting ownership to " + playerType);
                ulong clientId = NetworkManager.Singleton.LocalClientId;
                RequestOwnershipServerRpc(clientId);
                isTransfered = true;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestOwnershipServerRpc(ulong clientId)
        {
            Debug.Log("Requesting ownership to " + clientId);
            NetworkObject networkObject = GetComponent<NetworkObject>();
            networkObject.ChangeOwnership(clientId);
        }

    }
}