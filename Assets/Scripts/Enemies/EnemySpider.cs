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
            base.OnNetworkSpawn();
            EventManager.Instance.Subscribe(nameof(KnightAttackEvent), DoTakeDamage);
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe(nameof(KnightAttackEvent), DoTakeDamage);
            base.OnNetworkDespawn();
        }

        private void DoTakeDamage(EventBase baseEvent)
        {
            KnightAttackEvent e = baseEvent as KnightAttackEvent;
            // if gameobject is this
            if (e.other == gameObject)
            {
                gameObject.GetComponent<Animator>().Play("Take Damage");
                TakeDamageServerRpc(e.damage);
                Debug.Log("Spider::DoTakeDamage::" + e.other + " " + e.damage + ". Health Remain: " + Health);
            }
        }

    }
}