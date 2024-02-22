using Events;
using Items;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Players
{
    public class PrincessHealOrb : NetworkBehaviour
    {
        [SerializeField]
        private SphereCollider coll;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            coll.enabled = false;
        }

        private void Heal()
        {
            if(!IsLocalPlayer)
            {
                return;
            }
            float sphereRadius = coll.radius * transform.localScale.x;
            Vector3 sphereCenter = transform.position + coll.center;
            Debug.Log("Heal: " + sphereCenter + " " + sphereRadius);

            Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, sphereRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    HealPlayerServerRpc(hitCollider.gameObject.GetComponent<Player>().playerType, 10);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void HealPlayerServerRpc(ItemAccessbility playerType, int amount)
        {
            Debug.Log("HealPlayerServerRpc: " + playerType + " " + amount);
            HealPlayerClientRpc(playerType, amount);
        }

        [ClientRpc]
        private void HealPlayerClientRpc(ItemAccessbility playerType, int amount)
        {
            Debug.Log("HealPlayerClientRpc: " + playerType + " " + amount);
            new PlayerHealEvent(playerType, amount);
        }
    }
}