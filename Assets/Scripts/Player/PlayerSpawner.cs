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
    GameObject knightPrefab;

    [SerializeField]
    GameObject princessPrefab;

    [SerializeField]
    Transform knightSpawnPoint;

    [SerializeField]
    Transform princessSpawnPoint;

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId, ItemAccessbility playerType)
    {
        GameObject newPlayer;
        if (playerType == ItemAccessbility.knight)
            newPlayer = Instantiate(knightPrefab, knightSpawnPoint.position, knightSpawnPoint.rotation);
        else
            newPlayer = Instantiate(princessPrefab, princessSpawnPoint.position, princessSpawnPoint.rotation);
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

    private void OnSpawnPlayerEvent(BaseEvent ev)
    {
        SpawnPlayerEvent e = (SpawnPlayerEvent)ev;
        if (e != null)
        {
            if (e.playerId == 0)
            {
                SpawnPlayerServerRpc(e.playerId, ItemAccessbility.knight);
            }
            else
            {
                SpawnPlayerServerRpc(e.playerId, ItemAccessbility.princess);
            }
        }

    }
}

