using Events;
using Items;
using Managers;
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
            if (GameManager.Instance.LocalPlayer.playerType == ItemAccessbility.knight)
            {
                //SpawnKnightServerRpc();
                GameManager.Instance.LocalPlayer.transform.position = knightSpawnPoint.position;
            }
            else if (GameManager.Instance.LocalPlayer.playerType == ItemAccessbility.princess)
            {
                //SpawnPrincessServerRpc();
                GameManager.Instance.LocalPlayer.transform.position = princessSpawnPoint.position;
            }
        }

        [ServerRpc]
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

        [ServerRpc]
        private void SpawnPrincessServerRpc()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.GetComponent<Player>().playerType == ItemAccessbility.princess)
                {
                    Debug.Log("teleport");
                    go.transform.position = princessSpawnPoint.position;
                }
            }
        }
    }
}