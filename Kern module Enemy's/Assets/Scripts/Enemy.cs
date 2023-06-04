using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //view enemy
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject PlayerView;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canseeplayer;

    public bool delay = true;
    public bool NoSeeBoost = false;

    //ai
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //states
    public float sightRange;
    public bool playerInSightRange;

    private void Start()
    {
        //view enemy
        PlayerView = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
       
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
   
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
       
        if (NoSeeBoost)
        {
            agent.speed = 6;
        }
        //minimum of 20 distance to walk from standing point
        if( randomZ > - 20 && randomZ < 0)
        {
            randomZ = randomZ - 20; 
        }
        if (randomZ < 20 && randomZ > 0)
        {
            randomZ = randomZ + 20;
        }
        if (randomX > -20 && randomX < 0)
        {
            randomX = randomX - 20;
        }
        if (randomX < 20 && randomX > 0)
        {
            randomX = randomX + 20;
        }

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = 12;
    }

    private void Update()
    {
        //Check for sight
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        
        if (!canseeplayer && delay) Patroling();
        if (canseeplayer)
        {
            delay = false;
            ChasePlayer();
        }
        if (!delay && !canseeplayer)
        {
            StartCoroutine(WaitBeforePatrol());          
        }

        if (NoSeeBoost)
        {
            agent.speed = 10;
        }

        canseeplayer = true;
    }
    private IEnumerator WaitBeforePatrol()
    {
        canseeplayer = true;
        yield return new WaitForSeconds(20);
        canseeplayer = false;
        delay = true;
    }
    //view enemy
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldofViewCheck();
        }
    }

    private void FieldofViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canseeplayer = true;
                else
                    canseeplayer = false;
            }
            else
                canseeplayer= false;
        }
        else if (canseeplayer)
            canseeplayer= false;
    }
}
