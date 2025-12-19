using UnityEngine;
using UnityEngine.InputSystem;

public class SittingScript : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb; // Reference to freeze movement physics

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // --- 1. HANDLE SITTING (C Key) ---
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            animator.SetBool("IsSitting", true);
            
            // Freeze movement when sitting down
            if (rb != null) rb.isKinematic = true; 
        }
        
        if (Keyboard.current.cKey.wasReleasedThisFrame)
        {
            animator.SetBool("IsSitting", false);
            
            // Unfreeze movement when standing up
            if (rb != null) rb.isKinematic = false;
        }

        // --- 2. HANDLE EATING (E Key) ---
        // Assuming Eating is a toggle or a held action
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            animator.SetBool("IsEating", true);
        }
        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            animator.SetBool("IsEating", false);
        }

        // --- 3. HANDLE MOVEMENT & SPRINTING ---
        // We only allow movement logic if NOT sitting
        bool isSitting = animator.GetBool("IsSitting");
        
        if (!isSitting)
        {
            bool moveKeyPressed = Keyboard.current.wKey.isPressed || 
                                 Keyboard.current.aKey.isPressed || 
                                 Keyboard.current.sKey.isPressed || 
                                 Keyboard.current.dKey.isPressed ||
                                 Keyboard.current.upArrowKey.isPressed ||
                                 Keyboard.current.downArrowKey.isPressed ||
                                 Keyboard.current.leftArrowKey.isPressed ||
                                 Keyboard.current.rightArrowKey.isPressed;

            bool shiftPressed = Keyboard.current.shiftKey.isPressed;

            if (moveKeyPressed)
            {
                animator.SetBool("IsWalking", true);
                animator.SetBool("Sprinting", shiftPressed);
            }
            else
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("Sprinting", false);
            }
        }
        else
        {
            // Force movement booleans to false if the player is sitting
            animator.SetBool("IsWalking", false);
            animator.SetBool("Sprinting", false);
        }
    }
}