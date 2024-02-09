using Events;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;

public class HitDetection : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (player.IsLocalPlayer && other.gameObject.CompareTag("Enemy") && playerAnimator.GetBool("Attacking"))
        {
            Debug.Log("Hit");
            new KnightAttackEvent(other.gameObject);
        }

    }
}
