using UnityEngine;
using UnityEngine.InputSystem;

public class LeftDoor : MonoBehaviour
{
    public Vector3 openRotation = new Vector3(0, -90, 0); 
    public float speed = 2.0f;
    
    private Quaternion closedRot;
    private Quaternion openRot;
    private bool playerIsNear = false;

    void Start() {
        closedRot = transform.localRotation;
        openRot = Quaternion.Euler(openRotation);
    }

    void Update() {
        if (playerIsNear) {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRot, Time.deltaTime * speed);
        } else {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRot, Time.deltaTime * speed);
        }
    }

    // This detects the player entering
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerIsNear = true;
        }
    }

    // This detects the player leaving
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerIsNear = false;
        }
    }
}