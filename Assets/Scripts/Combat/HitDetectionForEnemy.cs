using Events;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetectionForEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator m_Animator;
    void Start()
    {
        m_Animator = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_Animator.GetBool("Attack"))
        {
            Debug.Log("Enemy Hit Player!");
            new EnemyAttackEvent(other.gameObject.GetComponentInParent<Player>().playerType);
        }
    }
}
