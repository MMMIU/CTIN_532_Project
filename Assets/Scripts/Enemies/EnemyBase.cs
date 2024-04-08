using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Enemies
{
    public class EnemyBase : NetworkBehaviour
    {
        public int enemy_uid;
        public NetworkVariable<EnemyDataItem> enemyDataItem = new();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            GetEnemyDataServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void GetEnemyDataServerRpc()
        {
            enemyDataItem.Value = EnemyLogic.Instance.GetEnemyData(enemy_uid);
            health.Value = enemyDataItem.Value.health;
        }

        [SerializeField]
        private NetworkVariable<int> health = new NetworkVariable<int>(0);
        public int Health
        {
            get => health.Value;
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void TakeDamageServerRpc(int damage)
        {
            if (health.Value <= 0)
            {
                return;
            }
            health.Value -= damage;
            if (health.Value <= 0)
            {
                DieClientRpc();
                if(TryGetComponent<QuestProgressModifier>(out var questProgressModifier))
                {
                    questProgressModifier.AddProgress();
                }
            }
        }

        [ClientRpc]
        public virtual void DieClientRpc()
        {
            Debug.Log("EnemyBase::DieClientRpc::" + enemyDataItem.Value.name);
            this.gameObject.GetComponent<Animator>().Play("Die");
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        [ClientRpc]
        public void SelfDestoryClientRpc()
        {
            Destroy(gameObject);
        }
    }
}