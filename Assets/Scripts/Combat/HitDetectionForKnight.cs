using Events;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;

public class HitDetectionForKnight : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Collider coll;

    private void Start()
    {
        coll.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.IsLocalPlayer && other.gameObject.CompareTag("Enemy") && playerAnimator.GetBool("Attacking"))
        {
            Debug.Log("Hit");
            new KnightAttackEvent(other.gameObject);
        }
    }

    public void EnableTrigger()
    {
        coll.enabled = true;
    }

    public void DisableTrigger()
    {
        coll.enabled = false;
    }
}
