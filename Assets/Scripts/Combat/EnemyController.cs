using Events;
using Items;
using Players;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : NetworkBehaviour
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
    bool alreadyAttacked;

    //States
    public float sightRange = 8f, attackRange = 1f;
    public bool targetInSightRange, targetInAttackRange;


    public Animator playerAnimator;
    public Animator selfAnimator;
    public int attacked = 0;
    #endregion

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players: " + players.Length);
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        Debug.Log("Agent enabled: " + agent.enabled);
        if (players.Length == 1)
        {
            targetPlayer = players[0].transform;
        }
        else
        {
            if (players[0].GetComponent<Player>().playerType == ItemAccessbility.princess)
            {
                targetPlayer = players[0].transform;
            }
            else
            {
                targetPlayer = players[1].transform;
            }
        }
        selfAnimator = this.GetComponent<Animator>();
        Debug.Log(targetPlayer);


        EventManager.Instance.Subscribe(nameof(KnightAttackEvent), ResetTargetPlayer);
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe(nameof(KnightAttackEvent), ResetTargetPlayer);
        base.OnNetworkDespawn();
    }
    void Update()
    {
        if (!IsServer||!IsSpawned||targetPlayer==null)
        {
            return;
        }
        //check for sight and attack range
        targetInSightRange = (transform.position - targetPlayer.position).magnitude < sightRange;
        targetInAttackRange = (transform.position - targetPlayer.position).magnitude < attackRange;


        if (!targetInSightRange && !targetInAttackRange) Patroling();
        if (targetInSightRange && !targetInAttackRange)
        {
            agent.SetDestination(targetPlayer.position);
            selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
        }
        if (targetInAttackRange && targetInSightRange) AttackTarget();
    }




    private void Patroling()
    {
        if (!IsServer)
        {
            return;
        }
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

    }

    private void AttackTarget()
    {
        if (!IsServer)
        {
            return;
        }
        //make sure enemy doesn't move
        agent.SetDestination(transform.position);
        selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));

        transform.LookAt(targetPlayer);

        if (!alreadyAttacked)
        {
            //Attack
            selfAnimator.Play("Stab Attack");
            selfAnimator.SetBool("Attack", true);

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


    //public void attackedByPlayer(GameObject attacker)
    //{
    //    if (!IsServer)
    //    {
    //        return;
    //    }
    //    targetPlayer = attacker.transform.root;
    //    transform.LookAt(targetPlayer);
    //    this.GetComponent<Animator>().Play("Take Damage");

    //}

    void Death()
    {
        if (!IsServer)
        {
            return;
        }
        Destroy(this.GetComponent<CapsuleCollider>());
        this.GetComponent<Animator>().Play("Death");
    }

    public void ResetTargetPlayer(BaseEvent baseE)
    {
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

            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}
}
