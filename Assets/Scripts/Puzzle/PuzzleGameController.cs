using Events;
using Items;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Puzzle
{
    public class PuzzleGameController : NetworkBehaviour
    {
        [SerializeField]
        Transform knightSpawnPoint;
        [SerializeField]
        Transform princessSpawnPoint;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            EventManager.Instance.Subscribe<PuzzleEnemyAttackEvent>(OnPuzzleEnemyAttack);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            EventManager.Instance.Unsubscribe<PuzzleEnemyAttackEvent>(OnPuzzleEnemyAttack);
        }

        private void OnPuzzleEnemyAttack(PuzzleEnemyAttackEvent e)
        {
            Debug.Log("PuzzleGameController::OnPuzzleEnemyAttack::" + e.playerType);
            if (e.playerType == ItemAccessbility.knight)
            {
                SpawnKnightServerRpc();
            }
            else if (e.playerType == ItemAccessbility.princess)
            {
                SpawnPrincessServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnKnightServerRpc()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.GetComponent<Player>().playerType == ItemAccessbility.knight)
                {
                    go.transform.position = knightSpawnPoint.position;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPrincessServerRpc()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.GetComponent<Player>().playerType == ItemAccessbility.princess)
                {
                    go.transform.position = princessSpawnPoint.position;
                }
            }
        }
    }
}