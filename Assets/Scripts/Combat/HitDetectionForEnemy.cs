using Events;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HitDetectionForEnemy : NetworkBehaviour
{
    // Start is called before the first frame update
    private Animator m_Animator;
    public override void OnNetworkSpawn()
    {
        m_Animator = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_Animator.GetBool("Attack"))
        {
            var type = other.gameObject.GetComponentInParent<Player>().playerType;
            if(type != GameManager.Instance.LocalPlayer.playerType)
            {
                return;
            }
            Debug.Log("Enemy Hit Player!");
            other.gameObject.transform.LookAt(transform.position);
            new EnemyAttackEvent(type);
        }
    }
}
