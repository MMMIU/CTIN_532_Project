using Events;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawnTrigger : NetworkBehaviour
    {
        [SerializeField]
        private NetworkVariable<bool> isActive = new NetworkVariable<bool>(true);

        [SerializeField]
        private Collider coll;

        [SerializeField]
        private int enemyId;

        [SerializeField]
        private int spawnPlace;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            coll = GetComponent<Collider>();
            isActive.OnValueChanged += OnActiveChanged;
        }

        public override void OnNetworkDespawn()
        {
            isActive.OnValueChanged -= OnActiveChanged;
            base.OnNetworkDespawn();
        }

        private void OnActiveChanged(bool previousValue, bool newValue)
        {
            coll.enabled = newValue;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                new EnemySpawnEvent(enemyId, spawnPlace);
                SetActiveServerRpc(false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetActiveServerRpc(bool value)
        {
            isActive.Value = value;
        }

    }
}
