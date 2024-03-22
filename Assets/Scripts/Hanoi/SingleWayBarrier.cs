using Items;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi
{
    public class SingleWayBarrier : MonoBehaviour
    {
        [SerializeField]
        ItemAccessbility playerType;

        [SerializeField]
        Collider coll;

        [SerializeField]
        bool allowPassAfterTrigger = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer && (playerType == ItemAccessbility.both || p.playerType == playerType))
                {
                    coll.isTrigger = allowPassAfterTrigger;
                }
            }
        }
    }
}