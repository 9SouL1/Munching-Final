using UnityEngine;
using UnityEngine.InputSystem; // Add this line!

public class MunchingScript : MonoBehaviour
{
    public FoodManager foodManager;
    private GameObject foodInRange;
    private GameObject heldFood;
    private bool isEating = false;

    void Update()
    {
        // Check if keyboard exists to avoid errors
        if (Keyboard.current == null) return;

        // GRAB: New Input System way for "Q"
        if (Keyboard.current.qKey.wasPressedThisFrame && foodInRange != null && heldFood == null)
        {
            GrabFood();
        }

        // EAT: New Input System way for "E"
        if (Keyboard.current.eKey.wasPressedThisFrame && heldFood != null && !isEating)
        {
            StartCoroutine(EatFood());
        }
    }

    void GrabFood()
    {
        heldFood = foodInRange;
        heldFood.transform.SetParent(this.transform); // Attach to player
        heldFood.transform.localPosition = new Vector3(0.2f, 0f, 0.25f); // Position in front
        foodInRange = null;
        Debug.Log("Food Grabbed!");
    }

    System.Collections.IEnumerator EatFood()
    {
        isEating = true;
        Debug.Log("Eating...");
        
        // Destroy the food being held
        Destroy(heldFood);
        heldFood = null;

        // Tell manager to spawn next one after 2 seconds
        yield return foodManager.StartCoroutine(foodManager.EatAndRespawnRoutine());
        
        isEating = false;
        Debug.Log("Done Eating. Next food spawned!");
    }

    // Detection Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            foodInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            foodInRange = null;
        }
    }
}