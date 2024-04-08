using Events;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HitDetectionForPuzzleEnemy : NetworkBehaviour
{
    // Start is called before the first frame update
    private Animator m_Animator;
    public PuzzleEnemyController m_PuzzleEnemyController;
    public override void OnNetworkSpawn()
    {
        m_Animator = transform.root.GetComponent<Animator>();
        m_PuzzleEnemyController = transform.root.GetComponent<PuzzleEnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Puzzle Enemt trigger entered");
        if (other.gameObject.CompareTag("Player") && m_Animator.GetBool("Attack"))
        {
            var type = other.gameObject.GetComponentInParent<Player>().playerType;
            if(type != GameManager.Instance.LocalPlayer.playerType)
            {
                return;
            }
            Debug.Log("Puzzle Enemy Hit Player!");
            m_PuzzleEnemyController.ChaseEndCheck();
            new PuzzleEnemyAttackEvent(type);
        }
    }
}
