using Events;
using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Players
{
    public partial class Player : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void InitPlayerServerRpc(string name)
        {
            Debug.Log("InitPlayerServerRpc: " + name);
            PlayerData oldValue = playerData.Value;
            playerData.Value.PlayerName = name;
            playerData.Value.playerHealth = 100;
            playerData.Value.playerMaxHealth = 100;
            playerData.Value.playerEnergy = 100;
            playerData.Value.playerMaxEnergy = 100;
            playerData.OnValueChanged?.Invoke(oldValue, playerData.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerTakeDamageServerRpc(int damage)
        {
            if (playerData.Value.playerDead)
            {
                Debug.Log("PlayerTakeDamageServerRpc: Player is dead");
                return;
            }
            Debug.Log("PlayerTakeDamageServerRpc: " + damage);
            PlayerData oldValue = playerData.Value;
            playerData.Value.playerHealth -= damage;
            if (playerData.Value.playerHealth <= 0)
            {
                playerData.Value.playerDead = true;
                PlayerDeadServerRpc();
            }
            playerData.OnValueChanged?.Invoke(oldValue, playerData.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerDeadServerRpc()
        {
            PlayerData oldValue = playerData.Value;
            playerData.Value.playerDead = true;
            playerData.OnValueChanged?.Invoke(oldValue, playerData.Value);
        }

        [ClientRpc]
        public void PlayerDeadClientRpc()
        {
            Debug.Log("PlayerDeadClientRpc");
            new PlayerDeadEvent(playerType);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerRespawnServerRpc()
        {
            PlayerData oldValue = playerData.Value;
            playerData.Value.playerHealth = playerData.Value.playerMaxHealth;
            playerData.Value.playerEnergy = playerData.Value.playerMaxEnergy;
            playerData.Value.playerDead = false;
            playerData.OnValueChanged?.Invoke(oldValue, playerData.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerHealServerRpc(int heal)
        {
            if (playerData.Value.playerDead)
            {
                return;
            }
            PlayerData oldValue = playerData.Value;
            playerData.Value.playerHealth += heal;
            if (playerData.Value.playerHealth > playerData.Value.playerMaxHealth)
            {
                playerData.Value.playerHealth = playerData.Value.playerMaxHealth;
            }
            playerData.OnValueChanged?.Invoke(oldValue, playerData.Value);
        }
    }
}

