using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class RaycastNPC : MonoBehaviour
{
    public string pathTag = "Tpath";
    public float speed = 5f;
    public float rotSpeed = 3f;
    public float obstacleRange = 3f; // How far ahead to look
    public float sideRayAngle = 30f; // Angle of the "whisker" rays

    private List<Transform> waypoints = new List<Transform>();
    private int currentIndex = 0;

    void Start() {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(pathTag);
        foreach (GameObject t in targets) waypoints.Add(t.transform);
        // Sort waypoints by name
        waypoints.Sort((a, b) => string.Compare(a.name, b.name));
    }

    void Update() {
        if (waypoints.Count == 0) return;

        MoveAndAvoid();
        
        // Check if reached waypoint
        if (Vector3.Distance(transform.position, waypoints[currentIndex].position) < 1.5f) {
            currentIndex = (currentIndex + 1) % waypoints.Count;
        }
    }

    void MoveAndAvoid() {
        Vector3 dir = (waypoints[currentIndex].position - transform.position).normalized;
        RaycastHit hit;

        // 1. Check center for obstacles
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, obstacleRange)) {
            // 2. Obstacle detected! Determine whether to turn left or right
            Vector3 leftRayDir = Quaternion.AngleAxis(-sideRayAngle, Vector3.up) * transform.forward;
            Vector3 rightRayDir = Quaternion.AngleAxis(sideRayAngle, Vector3.up) * transform.forward;

            // Choose the direction that is "more clear"
            if (Physics.Raycast(transform.position + Vector3.up, leftRayDir, obstacleRange)) {
                dir += transform.right; // Turn right if left is blocked
            } else {
                dir -= transform.right; // Turn left
            }
        }

        // Apply movement and rotation
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;

        // Visual debug lines
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * obstacleRange, Color.red);
    }
}