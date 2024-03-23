using Events;
using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Items;
using Players;
using Utils;
using Unity.VisualScripting;

namespace Enemies
{
    public class EnemySpawner : NetworkBehaviour
    {
        private static EnemySpawner instance;
        public static EnemySpawner Instance
        {
            get
            {
                return instance;
            }
        }

        [SerializeField]
        List<Pair<int, GameObject>> enemyPrefabs;

        [SerializeField]
        List<Transform> spawnPoints;
        public int spawnerID;

        public override void OnNetworkSpawn()
        {
            instance = this;
            EventManager.Instance.Subscribe(nameof(EnemySpawnEvent), OnSpawnEnemyEvent);
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe(nameof(EnemySpawnEvent), OnSpawnEnemyEvent);
            base.OnNetworkDespawn();
        }

        private void OnSpawnEnemyEvent(EventBase ev)
        {
            if (ev is EnemySpawnEvent enemySpawnEvent && enemySpawnEvent.playerType == GameManager.Instance.LocalPlayer.playerType && enemySpawnEvent.spawnerID == spawnerID)
            {
                SpawnEnemyServerRpc(enemySpawnEvent.enemyId, enemySpawnEvent.spawnPlace);
            }
        }

        private GameObject GetEnemyWithID(int enemyId)
        {
            foreach (var enemy in enemyPrefabs)
            {
                if (enemy.First == enemyId)
                {
                    return enemy.Second;
                }
            }
            return null;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEnemyServerRpc(int enemyId, int spawnPlace)
        {
            var enemyPrefab = GetEnemyWithID(enemyId);
            if (enemyPrefab == null)
            {
                Debug.LogError("Invalid enemy id: " + enemyId);
                return;
            }

            if (spawnPlace < 0 || spawnPlace >= spawnPoints.Count)
            {
                Debug.LogError("Invalid spawn place: " + spawnPlace);
                return;
            }

            Transform spawnPoint = spawnPoints[spawnPlace];
            Debug.Log("Spawning enemy " + enemyId + " at " + spawnPoint.position);
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            if(spawnerID == 2)
            {
                enemy.GetComponent<enemyController>().startTtargetSetToKnight();
            }
            NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
            networkObject.Spawn();
        }
    }

}