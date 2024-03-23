using Events;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawnTrigger : NetworkBehaviour
    {
        [SerializeField]
        bool devActive = false;

        [SerializeField]
        private NetworkVariable<bool> isActive;

        [SerializeField]
        private Collider coll;

        [SerializeField]
        private int enemyId;

        [SerializeField]
        private int spawnPlace;
        [SerializeField]
        private int spawnerID;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            coll = GetComponent<Collider>();
            isActive.OnValueChanged += OnActiveChanged;
            SetActiveServerRpc(true);
        }

        public override void OnNetworkDespawn()
        {
            isActive.OnValueChanged -= OnActiveChanged;
            base.OnNetworkDespawn();
        }

        private void OnActiveChanged(bool previousValue, bool newValue)
        {
            Debug.Log("OnActiveChanged: " + newValue);
            coll.enabled = newValue;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!(GameManager.Instance.DevMode && !devActive) && other.CompareTag("Player"))
            {
                SetActiveServerRpc(false);
                new EnemySpawnEvent(other.GetComponent<Player>().playerType, enemyId, spawnerID, spawnPlace);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetActiveServerRpc(bool value)
        {
            isActive.Value = value;
        }

    }
}
