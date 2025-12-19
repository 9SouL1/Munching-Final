using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NPCChaser : MonoBehaviour
{
    [Header("Detection Settings")]
    public AdvancedEatingController playerEatingScript;
    public float chaseDistance = 15f;
    public float catchDistance = 1.8f; // Increased slightly to prevent physics clipping
    public string gameOverScene = "GameOver";

    [Header("Wander Settings")]
    public float wanderRadius = 15f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    private Animator anim;
    private float wanderTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Find the player script automatically
        if (playerEatingScript == null)
            playerEatingScript = Object.FindFirstObjectByType<AdvancedEatingController>();

        // Force the first wander destination immediately
        wanderTimer = wanderInterval;
    }

    void Update()
    {
        if (playerEatingScript == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerEatingScript.transform.position);

        // 1. CHASE TRIGGER: Must be eating AND within sight range
        if (playerEatingScript.isEating && distanceToPlayer <= chaseDistance)
        {
            agent.SetDestination(playerEatingScript.transform.position);
            agent.speed = 5f; // Running speed
            if (anim != null) anim.SetBool("IsChasing", true);
        }
        else
        {
            // 2. AUTOMATIC WANDERING: If not chasing, walk randomly
            agent.speed = 2f; // Walking speed
            if (anim != null) anim.SetBool("IsChasing", false);
            DoWander();
        }

        // 3. GAME OVER TRIGGER
        // If NPC gets close enough, load the Game Over scene
        if (distanceToPlayer <= catchDistance)
        {
            Debug.Log("Caught the player!");
            SceneManager.LoadScene(gameOverScene);
        }

        // 4. ANIMATION SYNC
        if (anim != null)
        {
            // Update "Speed" float for Idle/Walk blend tree
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void DoWander()
    {
        wanderTimer += Time.deltaTime;

        // Pick a new spot if time is up OR we reached the current spot
        if (wanderTimer >= wanderInterval || (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending))
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            // Finds a valid point on the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            wanderTimer = 0f;
        }
    }
}