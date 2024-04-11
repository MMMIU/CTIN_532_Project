using Events;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCityFire : MonoBehaviour
{
    Collider fireCollider;

    private void Start()
    {
        fireCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fire Hit Something!");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Fire Hit Player!");
            Player player = other.GetComponentInChildren<Player>();
            if(!player.IsLocalPlayer)
            {
                return;
            }
            fireCollider.enabled = false;
            StartCoroutine(EnableFireCollider());
            new EnemyAttackEvent(player.playerType);
        }
    }

    IEnumerator EnableFireCollider()
    {
        yield return new WaitForSeconds(1f);
        fireCollider.enabled = true;
    }
}
