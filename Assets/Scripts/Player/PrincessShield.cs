using Enemies;
using Events;
using Items;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrincessShield : NetworkBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    Collider coll;

    void Start()
    {
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
    }

    public override void OnNetworkSpawn()
    {
        EventManager.Instance.Subscribe<PrincessSkillUpgradeEvent>(ShieldUp);
        EventManager.Instance.Subscribe<PrincessSkillDowngradeEvent>(ShieldDown);

    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe<PrincessSkillUpgradeEvent>(ShieldUp);
        EventManager.Instance.Unsubscribe<PrincessSkillDowngradeEvent>(ShieldDown);
    }

    public void ShieldUp(PrincessSkillUpgradeEvent baseEvent)
    {
        Debug.Log("ShieldUp");
        animator.SetBool("show", true);
        coll.enabled = true;
    }

    public void ShieldDown(PrincessSkillDowngradeEvent baseEvent)
    {
        Debug.Log("ShieldDown");
        animator.SetBool("show", false);
        coll.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player))
            {
                if (player.playerType == ItemAccessbility.knight)
                {
                    if (player.playerData.Value.playerHealth < player.playerData.Value.playerMaxHealth)
                    {
                        Debug.Log("HealPlayerServerRpc");
                        HealPlayerServerRpc(player.playerType, player.playerData.Value.playerMaxHealth - player.playerData.Value.playerHealth);
                    }
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player))
            {
                if (player.playerType == ItemAccessbility.knight)
                {
                    if (player.playerData.Value.playerDead)
                    {
                        Debug.Log("RespawnPlayerServerRpc");
                        RespawnPlayerServerRpc(player.playerType);
                    }
                }
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out EnemyBase enemy))
            {
                enemy.TakeDamageServerRpc(999);
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void HealPlayerServerRpc(ItemAccessbility playerType, float amount)
    {
        Debug.Log("HealPlayerServerRpc: " + playerType + " " + amount);
        HealPlayerClientRpc(playerType, amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnPlayerServerRpc(ItemAccessbility playerType)
    {
        Debug.Log("RespawnPlayerServerRpc: " + playerType);
        RespawnPlayerClientRpc(playerType);
    }

    [ClientRpc]
    private void HealPlayerClientRpc(ItemAccessbility playerType, float amount)
    {
        Debug.Log("HealPlayerClientRpc: " + playerType + " " + amount);
        new PlayerHealEvent(playerType, amount);
    }

    [ClientRpc]
    private void RespawnPlayerClientRpc(ItemAccessbility playerType)
    {
        Debug.Log("RespawnPlayerClientRpc: " + playerType);
        new PlayerRespawnEvent(playerType);
    }
}
