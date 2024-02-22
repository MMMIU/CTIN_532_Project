using Events;
using Items;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Enemies
{
    public class EnemySpider : EnemyBase
    {
        public override void OnNetworkSpawn()
        {
            if(!IsServer)
            {
                return;
            }
            base.OnNetworkSpawn();
            EventManager.Instance.Subscribe(nameof(KnightAttackEvent), DoTakeDamage);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
            {
                return;
            }
            EventManager.Instance.Unsubscribe(nameof(KnightAttackEvent), DoTakeDamage);
            base.OnNetworkDespawn();
        }

        private void DoTakeDamage(BaseEvent baseEvent)
        {
            if (!IsServer)
            {
                return;
            }
            KnightAttackEvent e = baseEvent as KnightAttackEvent;
            // if gameobject is this
            if (e.other == gameObject)
            {
                this.gameObject.GetComponent<Animator>().Play("Take Damage");
                TakeDamageServerRpc(e.damage);
                Debug.Log("Spider::DoTakeDamage::" + e.other + " " + e.damage + ". Health Remain: " + Health);
            }
        }

    }
}