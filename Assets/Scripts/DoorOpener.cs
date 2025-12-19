using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Door Objects")]
    public Transform leftDoor;  // Cube_14
    public Transform rightDoor; // Cube_10

    [Header("Rotation Settings")]
    public float speed = 3f;
    public float openAngle = 90f;
    
    private bool isPlayerNear = false;
    private Collider leftCollider;
    private Collider rightCollider;

    // Target Euler angles
    private Vector3 leftClosed = new Vector3(0, 0, 0);
    private Vector3 rightClosed = new Vector3(0, 0, 0);

    void Start()
    {
        // Automatically grab the colliders from the cubes
        if (leftDoor != null) leftCollider = leftDoor.GetComponent<Collider>();
        if (rightDoor != null) rightCollider = rightDoor.GetComponent<Collider>();
    }

    void Update()
    {
        // 1. Determine target angles
        Vector3 targetLeft = isPlayerNear ? new Vector3(0, -openAngle, 0) : leftClosed;
        Vector3 targetRight = isPlayerNear ? new Vector3(0, openAngle, 0) : rightClosed;

        // 2. Smoothly rotate the doors
        leftDoor.localRotation = Quaternion.Slerp(leftDoor.localRotation, Quaternion.Euler(targetLeft), Time.deltaTime * speed);
        rightDoor.localRotation = Quaternion.Slerp(rightDoor.localRotation, Quaternion.Euler(targetRight), Time.deltaTime * speed);

        // 3. THE FIX: Toggle 'Is Trigger' on the cubes themselves
        // When player is near, doors become 'ghosts' so you can walk through
        if (leftCollider != null) leftCollider.isTrigger = isPlayerNear;
        if (rightCollider != null) rightCollider.isTrigger = isPlayerNear;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            Debug.Log("Entry Detected: Opening Doors");
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            Debug.Log("Exit Detected: Closing Doors");
            isPlayerNear = false;
        }
    }
}