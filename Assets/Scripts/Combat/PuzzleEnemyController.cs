using Events;
using Items;
using Managers;
using Players;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class PuzzleEnemyController : NetworkBehaviour
{
    #region variables
    //AI Required
    public NavMeshAgent agent;
    public Transform targetPlayer;
    public Transform potentialTarget;
    bool targetFind;
    public LayerMask Ground, Player;

    //Patroling
    public Vector3[] walkPoints;
    public int currentWalkPointIndex;
    bool walkPointSet;
    public float walkPointRange = 5f;


    //States
    public float sightRange = 8f, attackRange = 1f;
    public bool targetInSightRange, targetInAttackRange;


    //Attacking
    public float timeBetweenAttacks = 2f;
    bool alreadyAttacked;
    public int attacked = 0;
    public bool hitByPlayer = false;

    public Animator playerAnimator;
    public Animator selfAnimator;
    public Transform LeftBound;
    public Transform RightBound;

    //public Vector3 PastPosition;
    //public float TimeIntervalForStuckCheck = 5f;
    [SerializeField]
    //private float TimeBetweenCheck;
    public float FakeChaseOffset = 0f;

    public float sightAngle = 60f;
    public bool chasing = false;
    public Transform raycastStartPoint;

    public float lostSightChaseTime = 1.5f;
    [SerializeField]
    private float lostStightChaseTimeCount; // in use for checking
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
                potentialTarget = targetPlayer;
            }
            else
            {
                if (players[0].GetComponent<Player>().playerType == ItemAccessbility.princess)
                {
                    targetPlayer = players[0].transform;
                    potentialTarget = players[1].transform;
                }
                else
                {
                    targetPlayer = players[1].transform;
                    potentialTarget = players[0].transform;
                }
            }
        }
        EventManager.Instance.Subscribe<KnightAttackEvent>(ResetTargetPlayer);
        Debug.Log(targetPlayer);

        LeftBound = GameObject.Find("Left Bound").transform;
        RightBound = GameObject.Find("Right Bound").transform;
        //PastPosition = this.transform.position;

        //TimeBetweenCheck = TimeIntervalForStuckCheck;
        currentWalkPointIndex = 0;
        var points = GameObject.FindGameObjectsWithTag("WayPoints");
        var wayPointsParentTransform = points[0].transform;
        walkPoints = new Vector3[wayPointsParentTransform.childCount];
        for (int i = 0; i < walkPoints.Length; i++)
        {
            walkPoints[i] = wayPointsParentTransform.GetChild(i).transform.position;
        }
        wayPointsParentTransform.gameObject.tag = "Untagged";
        lostStightChaseTimeCount = lostSightChaseTime;

    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe<KnightAttackEvent>(ResetTargetPlayer);
        base.OnNetworkDespawn();
    }
    void Update()
    {
        if (!IsServer || !IsSpawned || targetPlayer == null)
        {
            return;
        }
        //check for sight and attack range
        targetInSightRange = TargetInSightCheck();
        targetInAttackRange = (transform.position - targetPlayer.position).magnitude < attackRange;

        if (!targetInSightRange && lostStightChaseTimeCount <= 0f && !targetInAttackRange) Patroling();
        if ((targetInSightRange || lostStightChaseTimeCount > 0f) && !targetInAttackRange)
        {
            lostStightChaseTimeCount -=Time.deltaTime;
            agent.SetDestination(ChaseDestinationCreation());
            selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
        }
        if (targetInAttackRange && targetInSightRange) AttackTarget();
    }

    private bool TargetInSightCheck()
    {
        if (!IsServer)
        {
            return false;
        }
        var directionToTarget = targetPlayer.position - raycastStartPoint.position;
        var direction = directionToTarget;
        var directionToPotentialTarget = potentialTarget.position - raycastStartPoint.position;
        var directionPotential = directionToTarget;
        direction.y = 0;
        directionPotential.y = 0;

        RaycastHit hit;
        //check in sight range
        bool targetInSight = directionToTarget.magnitude < sightRange 
            && Vector3.Angle(raycastStartPoint.forward, direction.normalized) < sightAngle / 2;
        //check if it can be see
        if (targetInSight)
        {
            Physics.Raycast(raycastStartPoint.position, directionToTarget.normalized, out hit, sightRange);
            targetInSight = targetInSight && hit.transform.gameObject.CompareTag("Player");
        }
        //check in sight range
        bool potentialTargetInSight = directionToPotentialTarget.magnitude < sightRange 
            && Vector3.Angle(raycastStartPoint.forward, directionPotential.normalized) < sightAngle / 2;
        //check if it can be see
        if (potentialTargetInSight)
        {
            Physics.Raycast(raycastStartPoint.position, directionToPotentialTarget.normalized, out hit, sightRange);
            potentialTargetInSight = potentialTargetInSight && hit.transform.gameObject.CompareTag("Player");
        }

        if (potentialTargetInSight && targetInSight) 
        {
            if(directionToTarget.magnitude > directionToPotentialTarget.magnitude)
            {
                var temp = targetPlayer;
                targetPlayer = potentialTarget;
                potentialTarget = temp;
            }
            lostStightChaseTimeCount = lostSightChaseTime;
            return true;
        }
        else if (potentialTargetInSight)
        {
            var temp = targetPlayer;
            targetPlayer = potentialTarget;
            potentialTarget = temp;
            lostStightChaseTimeCount = lostSightChaseTime;
            return true;
        }
        else if (targetInSight)
        {
            lostStightChaseTimeCount = lostSightChaseTime;
            return true;
        }

        return false;
    }

    private Vector3 ChaseDestinationCreation()
    {
        //var destination = new Vector3(targetPlayer.position.x + FakeChaseOffset, targetPlayer.position.y, targetPlayer.position.z + FakeChaseOffset);
        //if (IsXOutBound(destination.x))
        //{
        //    destination.x -= 2* FakeChaseOffset;
        //}

        //if (IsZOutBound(destination.z))
        //{
        //    destination.z -= 2* FakeChaseOffset;
        //}

        //return destination;
        return targetPlayer.position;
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
            agent.SetDestination(walkPoints[currentWalkPointIndex]);
            selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
            //Debug.Log("Patroling");
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoints[currentWalkPointIndex];

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1.5f)
        {
            walkPointSet = false;
        }

        //if(TimeBetweenCheck > 0)
        //{
        //    TimeBetweenCheck -= Time.deltaTime;
        //}
        //else
        //{
        //    TimeBetweenCheck = TimeIntervalForStuckCheck;
        //    if((PastPosition - this.transform.position).magnitude < 1e-4)
        //    {
        //        SearchWalkPoint();
        //        agent.SetDestination(walkPoint);
        //        selfAnimator.SetFloat("Blend", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
        //    }
        //    PastPosition = this.transform.position;

        //}
    }

    private void SearchWalkPoint()
    {
        if (!IsServer)
        {
            return;
        }
        ////calculate random point in range
        //float randomZ = Random.Range(-walkPointRange, walkPointRange);
        //float randomX = Random.Range(-walkPointRange, walkPointRange);

        //var destinationX = transform.position.x + randomX;
        //var destinationZ = transform.position.z + randomZ;

        //while (IsXOutBound(destinationX)||IsZOutBound(destinationZ))
        //{
        //    randomZ = Random.Range(-walkPointRange, walkPointRange);
        //    randomX = Random.Range(-walkPointRange, walkPointRange);

        //    destinationX = transform.position.x + randomX;
        //    destinationZ = transform.position.z + randomZ;

        //}

        //walkPoint = new Vector3(destinationX, transform.position.y, destinationZ);
        currentWalkPointIndex += 1;
        if(currentWalkPointIndex >= walkPoints.Length)
        {
            currentWalkPointIndex = 0;
        }

        //TimeBetweenCheck = TimeIntervalForStuckCheck;
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
        if (!alreadyAttacked && !hitByPlayer)
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

    public void GotHit()
    {
        //selfAnimator.SetBool("Attack", false);
        hitByPlayer = true;
    }

    public void HitReactionDone()
    {
        hitByPlayer = false;
    }


    private bool IsXOutBound(float x)
    {
        return x < LeftBound.position.x || x > RightBound.position.x;
    }
    private bool IsZOutBound(float z)
    {
        return z < LeftBound.position.z || z > RightBound.position.z;
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

    [ServerRpc(RequireOwnership = false)]
    void DeathServerRpc()
    {
        if (!IsServer)
        {
            return;
        }
        GetComponent<CapsuleCollider>().enabled = false;
        DeathAnimClientRpc();
    }

    [ClientRpc]
    void DeathAnimClientRpc()
    {
        selfAnimator?.Play("Death");
    }

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

            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}

}
