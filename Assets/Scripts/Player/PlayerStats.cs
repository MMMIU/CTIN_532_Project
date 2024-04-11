using Events;
using Items;
using Managers;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Players
{
    public partial class Player : NetworkBehaviour
    {
        private void OnEnemyAttack(EnemyAttackEvent e)
        {
            if (IsLocalPlayer && playerType == e.playerType)
            {
                Debug.Log("OnEnemyAttack: " + e.damage);
                PlayerTakeDamageServerRpc(e.damage);
            }
        }

        private void OnPlayerHeal(PlayerHealEvent e)
        {
            if (IsLocalPlayer && playerType == e.playerType)
            {
                Debug.Log("OnPlayerHeal: " + e.amount);
                PlayerHealServerRpc(e.amount);
            }
        }

        private void OnPlayerDead(PlayerDeadEvent e)
        {
            if (IsLocalPlayer && playerType == e.playerType)
            {
                Debug.Log("OnPlayerDead: " + e.playerType);
            }
        }

        private void OnPlayerRespawn(PlayerRespawnEvent e)
        {
            if (IsLocalPlayer && playerType == e.playerType)
            {
                Debug.Log("OnPlayerRespawn: " + e.playerType);
                PlayerRespawnServerRpc();
            }
        }


        [ServerRpc]
        public void InitPlayerServerRpc(string name)
        {
            Debug.Log("InitPlayerServerRpc: " + name);
            playerData.Value = new()
            {
                PlayerName = name,
                playerType = playerType,
                playerHealth = playerType == ItemAccessbility.knight ? 100 : 50,
                playerMaxHealth = playerType == ItemAccessbility.knight ? 100 : 50,
                playerEnergy = playerType == ItemAccessbility.knight ? 50 : 100,
                playerMaxEnergy = playerType == ItemAccessbility.knight ? 50 : 100
            };
            playerData.SetDirty(true);
            playerData.OnValueChanged?.Invoke(null, playerData.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerTakeDamageServerRpc(int damage)
        {
            if (playerData.Value.playerDead)
            {
                Debug.Log("PlayerTakeDamageServerRpc: Player is dead");
                return;
            }
            if(playerData.Value.playerUnstoppable)
            {
                Debug.LogWarning("PlayerTakeDamageServerRpc: Player is unstoppable");
                return;
            }
            Debug.Log("PlayerTakeDamageServerRpc: " + playerData.Value.PlayerName + " " + damage);
            playerData.Value.playerHealth -= damage;
            if (playerData.Value.playerHealth <= 0)
            {
                playerData.Value.playerDead = true;
                PlayerDeadClientRpc(playerType);
            }
            Debug.Log("PlayerTakeDamageServerRpc: " + playerData.Value.PlayerName + " " + playerData.Value.playerHealth);
            playerData.SetDirty(true);
            playerData.OnValueChanged?.Invoke(null, playerData.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetGameOverServerRpc(bool gameover)
        {
            GameManager.Instance.gameover |= gameover;
        }

        [ClientRpc]
        public void PlayerDeadClientRpc(ItemAccessbility playerType)
        {
            // if player is princess, game over
            if (playerType == ItemAccessbility.princess)
            {
                new PlayerDeadEvent(playerType);
                UIManager.Instance.OpenPanel<UIFallen>(playerType);
                GameManager.Instance.gameover = true;
                SetGameOverServerRpc(true);
                return;
            }
            // if local player is knight, die
            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Player>().playerType == ItemAccessbility.knight)
            {
                new PlayerDeadEvent(playerType);
                UIManager.Instance.OpenPanel<UIFallen>(playerType);
            }
            // if local player is princess, game over or pop up message according to quest
            else
            {
                if (QuestManager.Instance.CheckTaskExist(6, 1))
                {
                    UIManager.Instance.OpenPanel<UIPopUpBar>("Knight has been fallen...");
                    return;
                }
                else
                {
                    UIManager.Instance.OpenPanel<UIFallen>(playerType);
                    GameManager.Instance.gameover = true;
                    SetGameOverServerRpc(true);
                }

            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerRespawnServerRpc()
        {
            playerData.Value.playerHealth = playerData.Value.playerMaxHealth;
            playerData.Value.playerEnergy = playerData.Value.playerMaxEnergy;
            playerData.Value.playerDead = false;
            playerData.SetDirty(true);
            playerData.OnValueChanged?.Invoke(null, playerData.Value);
            PlayerRespawnClientRpc(playerData.Value.playerType);
        }

        [ClientRpc]
        public void PlayerRespawnClientRpc(ItemAccessbility playerType)
        {
            Debug.Log("PlayerRespawnClientRpc: " + playerType);
            UIManager.Instance.Close<UIFallen>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerHealServerRpc(float heal)
        {
            Debug.Log("PlayerHealServerRpc: " + heal);
            if (playerData.Value.playerDead)
            {
                return;
            }
            if (playerData.Value.playerHealth == playerData.Value.playerMaxHealth)
            {
                return;
            }
            playerData.Value.playerHealth += heal;
            PlayHealParticleClientRpc();
            if (playerData.Value.playerHealth > playerData.Value.playerMaxHealth)
            {
                playerData.Value.playerHealth = playerData.Value.playerMaxHealth;
            }
            playerData.SetDirty(true);
            playerData.OnValueChanged?.Invoke(null, playerData.Value);
        }

        [ClientRpc]
        public void PlayHealParticleClientRpc()
        {
            ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
            particleSystem.Play();
        }
    }
}

