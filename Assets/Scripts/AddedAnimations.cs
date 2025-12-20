using UnityEngine;
using UnityEngine.InputSystem;

public sealed class AddedAnimations : MonoBehaviour
{
    private Animator animator;

    [Header("Animator Parameter Names")]
    [SerializeField] private string walkingParam = "IsWalking";
    [SerializeField] private string sprintingParam = "Sprinting";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;

        // If no keyboard is found, we force everything to false for safety
        if (kb == null) 
        {
            StopAnimations();
            return;
        }

        // 1. Check for any movement key
        bool movePressed = kb.wKey.isPressed || kb.sKey.isPressed || 
                           kb.aKey.isPressed || kb.dKey.isPressed ||
                           kb.upArrowKey.isPressed || kb.downArrowKey.isPressed || 
                           kb.leftArrowKey.isPressed || kb.rightArrowKey.isPressed;

        // 2. Check for Shift
        bool shiftPressed = kb.shiftKey.isPressed;

        // 3. Apply Logic
        if (movePressed)
        {
            animator.SetBool(walkingParam, true);
            
            // Sprinting only stays true if shift is held AND we are moving
            animator.SetBool(sprintingParam, shiftPressed);
        }
        else
        {
            // IF NO MOVEMENT KEYS ARE HELD:
            StopAnimations();
        }
    }

    private void StopAnimations()
    {
        animator.SetBool(walkingParam, false);
        animator.SetBool(sprintingParam, false);
    }
}