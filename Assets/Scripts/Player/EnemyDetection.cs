using Events;
using Players;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyDetection : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private SphereCollider coll;
    private bool switched = false;
    private float selfCheckTimeInterval = 2f;
    private float timer;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        EventManager.Instance.Subscribe<EnemyChaseStart>(ChaseStart);
        EventManager.Instance.Subscribe<EnemyChaseEnd>(ChaseStop);
        timer = selfCheckTimeInterval;
        coll.enabled = false;
    }
    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe<EnemyChaseStart>(ChaseStart);
        EventManager.Instance.Unsubscribe<EnemyChaseEnd>(ChaseStop);
        base.OnNetworkDespawn();
    }

    private void ChaseStop(EnemyChaseEnd end)
    {
        EnemyCheck();
    }

    private void ChaseStart(EnemyChaseStart start)
    {
        EnemyCheck();
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            EnemyCheck();
            timer = selfCheckTimeInterval;
        }
    }

    public void EnemyCheck()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        float sphereRadius = coll.radius * transform.localScale.x;
        Vector3 sphereCenter = transform.position + coll.center;

        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, sphereRadius, 1<<LayerMask.NameToLayer("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                //Debug.Log("Enemy Detection found enemy");
                var enemyStatus = hitCollider.gameObject.GetComponent<EnemyController>();
                //Debug.Log(enemyStatus);
                //Debug.Log(enemyStatus.Chasing);
                if (enemyStatus)
                {
                    if (enemyStatus.Chasing)
                    {
                        switched = true;
                        break;
                    }
                }
                var puzzleEnemyStatus = hitCollider.gameObject.GetComponent<PuzzleEnemyController>();
                if (puzzleEnemyStatus)
                {
                    if (puzzleEnemyStatus.Chasing)
                    {
                        switched = true;
                        break;
                    }
                }
            }
        }
        if (switched)
        {
            BackgroundMusicManager.instance.SwitchToBattleBGM();
            switched = false;
        }
        else
        {
            BackgroundMusicManager.instance.SwitchToOutsideBattleBGM();
        }
    }
}
