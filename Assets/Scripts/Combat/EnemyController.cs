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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players: " + players.Length);
        agent = GetComponent<NavMeshAgent>();
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
    }

    void Update()
    {
        //check for sight and attack range
        targetInSightRange = (transform.position - targetPlayer.position).magnitude < sightRange;
        targetInAttackRange = (transform.position - targetPlayer.position).magnitude < attackRange;


        if (!targetInSightRange && !targetInAttackRange) Patroling();
        if (targetInSightRange && !targetInAttackRange) ChasingServerRpc();
        if (targetInAttackRange && targetInSightRange) AttackTarget();
    }




    private void Patroling()
    {
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
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        walkPointSet = true;

    }

    [ServerRpc(RequireOwnership = false)]
    private void ChasingServerRpc()
    {
        agent.SetDestination(targetPlayer.position);
        selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
    }

    private void AttackTarget()
    {
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
        alreadyAttacked = false;
    }


    public void attackedByPlayer(GameObject attacker)
    {
        targetPlayer = attacker.transform.root;
        transform.LookAt(targetPlayer);
        this.GetComponent<Animator>().Play("HitReaction");
        attacked++;
        if (attacked > 2)
        {
            Death();
        }

    }

    void Death()
    {
        Destroy(this.GetComponent<CapsuleCollider>());
        this.GetComponent<Animator>().Play("Death");
    }

    void Destory()
    {
        Destroy(gameObject);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}
}
