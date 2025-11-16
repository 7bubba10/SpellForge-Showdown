using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 20f;
    public float stoppingDistance = 2.5f;

    [Header("Wandering")]
    public float idleWanderRadius = 5f;
    public float wanderDelay = 3f;

    [Header("Combat")]
    public int damage = 10;
    public float attackCooldown = 1.0f;

    private float attackTimer = 0f;

    private Transform target;
    private NavMeshAgent agent;
    private EnemyHealth myHealth;
    private float wanderTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        myHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        wanderTimer = wanderDelay;
    }

    private void Update()
    {
        if (myHealth == null) return;

        // Cooldown timer
        attackTimer -= Time.deltaTime;

        // No player?
        if (target == null)
        {
            Wander();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // Player within detection range → chase
        if (distance <= detectionRange)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(target.position);

            // Check if enemy can damage player
            if (distance <= stoppingDistance + 0.5f)
            {
                TryAttackPlayer();
            }
        }
        else
        {
            Wander();
        }
    }

    private void TryAttackPlayer()
    {
        if (attackTimer > 0f) return; // still cooling down

        if (target.TryGetComponent<Health>(out var hp))
        {
            hp.TakeDamage(damage);
        }

        attackTimer = attackCooldown;
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            Vector3 randomDir = Random.insideUnitSphere * idleWanderRadius;
            randomDir += transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDir, out hit, idleWanderRadius, NavMesh.AllAreas);

            agent.stoppingDistance = 0f;
            agent.SetDestination(hit.position);

            wanderTimer = wanderDelay;
        }
    }
}