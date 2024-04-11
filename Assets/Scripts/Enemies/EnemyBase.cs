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
            health.Value -= damage;
            if (health.Value <= 0)
            {
                health.Value = 0;
                gameObject.GetComponent<Animator>().Play("Die");
                gameObject.GetComponent<Animator>().SetTrigger("die");
                if (TryGetComponent<QuestProgressModifier>(out var questProgressModifier))
                {
                    questProgressModifier.AddProgress();
                }
            }
        }

        [ClientRpc]
        public void SelfDestoryClientRpc()
        {
            Destroy(gameObject);
        }
    }
}