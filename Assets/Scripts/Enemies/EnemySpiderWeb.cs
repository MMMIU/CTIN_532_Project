using Events;
using Items;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Enemies
{
    public class EnemySpiderWeb : EnemyBase
    {
        [SerializeField]
        private ItemSpiderWeb_1 itemSpiderWeb_1;

        [SerializeField]
        private Animator animator;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            EventManager.Instance.Subscribe<KnightAttackEvent>(DoTakeDamage);
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe<KnightAttackEvent>(DoTakeDamage);
            base.OnNetworkDespawn();
        }

        private void DoTakeDamage(EventBase baseEvent)
        {
            KnightAttackEvent e = baseEvent as KnightAttackEvent;
            // if gameobject is this
            if (e.other == gameObject)
            {
                Debug.Log("SpiderWeb::DoTakeDamage::" + e.other + " " + e.damage);
                DoTakeDamage(e.damage);
            }

        }

        public void DoTakeDamage(int damage)
        {
            if (!itemSpiderWeb_1.Interactable.Value)
            {
                return;
            }
            base.TakeDamageServerRpc(damage);
        }

        [ClientRpc]
        public override void DieClientRpc()
        {
            animator.SetTrigger("die");
        }

        public void OnDieAnimationEnd()
        {
            Destroy(gameObject);
        }
    }
}