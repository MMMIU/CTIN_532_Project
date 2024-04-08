using Events;
using Items;
using Players;
using Quest;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    #region variables
    //AI Required
    public NavMeshAgent agent;
    public Transform targetPlayer;
    bool targetFind;
    public LayerMask Ground, Knight, Princess;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 5f;

    //Attacking
    public float timeBetweenAttacks = 2f;
    private bool attacking = false;
    bool alreadyAttacked;

    //States
    public float sightRange = 8f, attackRange = 1f;
    public bool targetInSightRange, targetInAttackRange;


    public Animator playerAnimator;
    public Animator selfAnimator;
    public int attacked = 0;

    public bool hitByPlayer = false;
    public ItemAccessbility targetType = ItemAccessbility.princess;
    public Vector3 PastPosition;
    public float TimeIntervalForStuckCheck = 2f;
    [SerializeField]
    private float TimeBetweenCheck;
    [SerializeField]
    public bool Chasing = false;

    [SerializeField]
    private CapsuleCollider Horn;
    public bool EnemyDied = false;
    #endregion

    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        selfAnimator = this.GetComponent<Animator>();
        if (IsServer)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("Number of players: " + players.Length);
            agent.enabled = true;
            Debug.Log("Agent enabled: " + agent.enabled);
            if (players.Length == 1)
            {
                targetPlayer = players[0].transform;
            }
            else
            {
                if (players[0].GetComponent<Player>().playerType == targetType)
                {
                    targetPlayer = players[0].transform;
                }
                else
                {
                    targetPlayer = players[1].transform;
                }
            }
        }
        EventManager.Instance.Subscribe<KnightAttackEvent>(ResetTargetPlayer);
        EventManager.Instance.Subscribe<PlayerDeadEvent>(TargetRestPlayerDead);
        Debug.Log(targetPlayer);
        PastPosition = this.transform.position;

        TimeBetweenCheck = TimeIntervalForStuckCheck;
    }


    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe<KnightAttackEvent>(ResetTargetPlayer);
        EventManager.Instance.Unsubscribe<PlayerDeadEvent>(TargetRestPlayerDead);
        base.OnNetworkDespawn();
        ChaseEndCheck();
    }
    void Update()
    {
        if (!IsServer || !IsSpawned || EnemyDied)
        {
            return;
        }
        //check for sight and attack range
        if (targetPlayer != null)
        {
            targetInSightRange = (transform.position - targetPlayer.position).magnitude < sightRange;
            targetInAttackRange = (transform.position - targetPlayer.position).magnitude < attackRange;
        }
        else
        {
            targetInSightRange = false; 
            targetInAttackRange = false;
        }


        if (!targetInSightRange && !targetInAttackRange) Patroling();
        if (targetInSightRange && !targetInAttackRange)
        {
            ChaseStartCheck();
            agent.SetDestination(targetPlayer.position);
            selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
        }
        if (targetInAttackRange && targetInSightRange) AttackTarget();
    }

    public void ChaseStartCheck()
    {
        if (!IsServer)
        {
            return;
        }
        if (!Chasing)
        {
            Chasing = true;
            ChaseStartSycnClientRpc();
            new EnemyChaseStart();
        }
    }

    public void ChaseEndCheck()
    {
        if (!IsServer)
        {
            return;
        }
        if (Chasing)
        {
            Chasing = false;
            new EnemyChaseEnd();
            ChaseEndSycnClientRpc();
        }
    }


    [ClientRpc]
    void ChaseEndSycnClientRpc()
    {
        Chasing = false;
        new EnemyChaseEnd();
    }

    [ClientRpc]
    void ChaseStartSycnClientRpc()
    {
        Chasing = true;
        new EnemyChaseStart();
    }

    private void Patroling()
    {
        if (!IsServer)
        {
            return;
        }
        ChaseEndCheck();
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }


        if (TimeBetweenCheck > 0)
        {
            TimeBetweenCheck -= Time.deltaTime;
        }
        else
        {
            TimeBetweenCheck = TimeIntervalForStuckCheck;
            if ((PastPosition - this.transform.position).magnitude < 1e-4)
            {
                SearchWalkPoint();
                agent.SetDestination(walkPoint);
                selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
            }
            PastPosition = this.transform.position;

        }
        //Debug.Log("patrol");
    }

    private void SearchWalkPoint()
    {
        if (!IsServer)
        {
            return;
        }
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        walkPointSet = true;
        TimeBetweenCheck = TimeIntervalForStuckCheck;

    }

    private void AttackTarget()
    {
        if (!IsServer)
        {
            return;
        }
        ChaseStartCheck();
        //make sure enemy doesn't move
        agent.SetDestination(transform.position);
        selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));

        if (!attacking)
        {
            transform.LookAt(targetPlayer);
        }

        if (!alreadyAttacked && !hitByPlayer)
        {
            //Attack
            selfAnimator.Play("Stab Attack");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        if (!IsServer)
        {
            return;
        }
        alreadyAttacked = false;
    }

    public void GotHit()
    {
        //selfAnimator.SetBool("Attack", false);
        hitByPlayer = true;
    }

    public void HitReactionDone()
    {
        hitByPlayer = false;
    }

    public void SetAttackTrue()
    {
        selfAnimator.SetBool("Attack", true);
        Horn.enabled = true;
        attacking = true;
    }
    
    public void SetAttackFalse()
    {
        selfAnimator.SetBool("Attack", false);
        Horn.enabled = false;
        attacking = false;
    }

    public void EnemyDie()
    {
        EnemyDied = true;
    }


    public void HitByPlayer()
    {
        if (hitByPlayer) return;
        selfAnimator.Play("Take Damage");
    }

    //[ServerRpc(RequireOwnership = false)]
    //void DeathServerRpc()
    //{
    //    if (!IsServer)
    //    {
    //        return;
    //    }
    //    GetComponent<CapsuleCollider>().enabled = false;
    //    DeathAnimClientRpc();
    //}

    //[ClientRpc]
    //void DeathAnimClientRpc()
    //{
    //    selfAnimator?.Play("Die");
    //    if(TryGetComponent(out QuestProgressModifier questProgressModifier))
    //    {
    //        questProgressModifier.AddProgress();
    //    }
    //}

    public void ResetTargetPlayer(EventBase baseE)
    {
        if (!IsServer)
        {
            return;
        }

        KnightAttackEvent e = baseE as KnightAttackEvent;
        if (e.other == gameObject)
        {
            if (targetPlayer.gameObject.GetComponent<Player>().playerType != ItemAccessbility.knight)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length == 1)
                {
                    targetPlayer = players[0].transform;
                }
                else
                {
                    if (players[0].GetComponent<Player>().playerType == ItemAccessbility.knight)
                    {
                        targetPlayer = players[0].transform;
                    }
                    else
                    {
                        targetPlayer = players[1].transform;
                    }
                }
                playerAnimator = targetPlayer.gameObject.GetComponent<Animator>();
                targetType = targetPlayer.GetComponent<Player>().playerType;

            }
        }
    }
    private void TargetRestPlayerDead(PlayerDeadEvent baseEvent)
    {
        if (!IsServer)
        {
            return;
        }

        if(targetType == baseEvent.playerType)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 1)
            {
                targetPlayer = null;
                playerAnimator = null;
            }
            else
            {
                var player1Dead = players[0].GetComponent<Player>().playerData.Value.playerDead;
                var player2Dead = players[1].GetComponent<Player>().playerData.Value.playerDead;
                if (player1Dead && player2Dead)
                {
                    targetPlayer = null;
                    playerAnimator = null;
                }
                else if(player1Dead)
                {
                    targetPlayer = players[1].transform;
                    playerAnimator = targetPlayer.gameObject.GetComponent<Animator>();
                    targetType = targetPlayer.GetComponent<Player>().playerType;
                }
                else if(player2Dead)
                {
                    targetPlayer = players[0].transform;
                    playerAnimator = targetPlayer.gameObject.GetComponent<Animator>();
                    targetType = targetPlayer.GetComponent<Player>().playerType;
                }
                else
                {
                    Debug.LogError("Something wrong with playerDead event");
                }
            }
        }
    }

    public void startTtargetSetToKnight()
    {
        targetType = ItemAccessbility.knight;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}
}
