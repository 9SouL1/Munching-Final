using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NPCChaser : MonoBehaviour
{
    [Header("Detection Settings")]
    public AdvancedEatingController playerEatingScript;
    public float chaseDistance = 15f;
    public float catchDistance = 1.8f;
    public string gameOverScene = "UIScene";

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 5f;
    public float wanderRadius = 15f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    private Animator anim;
    private float wanderTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (playerEatingScript == null)
            playerEatingScript = Object.FindFirstObjectByType<AdvancedEatingController>();

        wanderTimer = wanderInterval;
    }

    void Update()
    {
        if (playerEatingScript == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerEatingScript.transform.position);

        // 1. STATE LOGIC: CHASE vs WANDER
        bool isPlayerEating = playerEatingScript.isEating;

        if (isPlayerEating && distanceToPlayer <= chaseDistance)
        {
            // CHASE STATE
            agent.speed = chaseSpeed;
            agent.SetDestination(playerEatingScript.transform.position);

            // Set the Bool for the transition to the separate "Chase" state
            if (anim != null) anim.SetBool("IsChasing", true);
        }
        else
        {
            // WANDER STATE (Walking randomly)
            agent.speed = walkSpeed;
            if (anim != null) anim.SetBool("IsChasing", false);
            DoWander();
        }

        // 2. GAME OVER TRIGGER
        if (distanceToPlayer <= catchDistance)
        {
            SceneManager.LoadScene(gameOverScene);
        }

        // 3. ANIMATION SYNC (This controls the Blend Tree)
        if (anim != null)
        {
            // Using desiredVelocity ensures the animation starts as soon as the agent starts to turn or move
            float currentMoveSpeed = agent.desiredVelocity.magnitude;

            // This updates the 'Speed' parameter in your Blend Tree
            anim.SetFloat("Speed", currentMoveSpeed);
        }
    }

    void DoWander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval || (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending))
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            wanderTimer = 0f;
        }
    }
}