using Events;
using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Items;
using Players;
using Utils;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    ItemAccessbility hostPlayerType;

    [SerializeField]
    GameObject knightPrefab;

    [SerializeField]
    GameObject princessPrefab;

    [SerializeField]
    Transform knightSpawnPoint;

    [SerializeField]
    Transform princessSpawnPoint;

    [SerializeField]
    bool useDevSpawnPoints = false;

    [SerializeField]
    Transform devKnightSpawnPoint;

    [SerializeField]
    Transform devPrincessSpawnPoint;

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId, ItemAccessbility playerType)
    {
        GameObject newPlayer;
        if (playerType == ItemAccessbility.knight)
        {
            if (useDevSpawnPoints)
            {
                Debug.Log("Spawning knight at: " + devKnightSpawnPoint.position);
                newPlayer = Instantiate(knightPrefab, devKnightSpawnPoint.position, devKnightSpawnPoint.rotation);
            }
            else
            {
                Debug.Log("Spawning knight at: " + knightSpawnPoint.position);
                newPlayer = Instantiate(knightPrefab, knightSpawnPoint.position, knightSpawnPoint.rotation);
            }
        }
        else
        {
            if (useDevSpawnPoints)
            {
                Debug.Log("Spawning princess at: " + devPrincessSpawnPoint.position);
                newPlayer = Instantiate(princessPrefab, devPrincessSpawnPoint.position, devPrincessSpawnPoint.rotation);
            }
            else
            {
                Debug.Log("Spawning princess at: " + princessSpawnPoint.position);
                newPlayer = Instantiate(princessPrefab, princessSpawnPoint.position, princessSpawnPoint.rotation);
            }
        }
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        //newPlayer.GetComponent<Player>().playerData.Value.ResetAll(true);
        newPlayer.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        EventManager.Instance.Subscribe(nameof(SpawnPlayerEvent), OnSpawnPlayerEvent);
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe(nameof(SpawnPlayerEvent), OnSpawnPlayerEvent);
        base.OnNetworkDespawn();
    }

    private void OnSpawnPlayerEvent(EventBase ev)
    {
        SpawnPlayerEvent e = (SpawnPlayerEvent)ev;
        if (e != null)
        {
            if (e.playerId == 0)
            {
                if (hostPlayerType == ItemAccessbility.knight)
                    SpawnPlayerServerRpc(NetworkManager.LocalClientId, ItemAccessbility.knight);
                else
                    SpawnPlayerServerRpc(NetworkManager.LocalClientId, ItemAccessbility.princess);
            }
            else
            {
                if (hostPlayerType == ItemAccessbility.knight)
                    SpawnPlayerServerRpc(e.playerId, ItemAccessbility.princess);
                else
                    SpawnPlayerServerRpc(e.playerId, ItemAccessbility.knight);
            }
        }

    }
}

