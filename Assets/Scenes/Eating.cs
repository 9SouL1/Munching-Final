using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AdvancedEatingController : MonoBehaviour
{
    [Header("UI & Animation")]
    public Animator anim;
    public Slider eatingBar;
    public GameObject barContainer;

    [Header("Bite Settings")]
    public int totalBitesRequired = 5;
    public float timePerBite = 1.0f;

    private bool isEating = false;
    // Moved outside the routine so it persists
    private int currentBitesDone = 0;

    void Update()
    {
        // Start eating if "E" is held and we aren't finished yet
        if (Keyboard.current.eKey.wasPressedThisFrame && !isEating && currentBitesDone < totalBitesRequired)
        {
            StartCoroutine(EatRoutine());
        }
    }

    IEnumerator EatRoutine()
    {
        isEating = true;

        if (barContainer != null) barContainer.SetActive(true);
        if (anim != null) anim.SetBool("IsEating", true);

        while (currentBitesDone < totalBitesRequired)
        {
            float biteTimer = 0;

            while (biteTimer < timePerBite)
            {
                // Check if user let go
                if (!Keyboard.current.eKey.isPressed)
                {
                    StopEatingEarly();
                    yield break;
                }

                biteTimer += Time.deltaTime;

                if (eatingBar != null)
                {
                    // Keep the progress bar accurate to saved progress
                    float totalProgress = (currentBitesDone + (biteTimer / timePerBite)) / totalBitesRequired;
                    eatingBar.value = totalProgress;
                }

                yield return null;
            }

            currentBitesDone++;
            Debug.Log("Finished bite: " + currentBitesDone);
        }

        CompleteEating();
    }

    void StopEatingEarly()
    {
        isEating = false;
        // We REMOVED currentBitesDone = 0; so it stays at its current value
        if (barContainer != null) barContainer.SetActive(false);
        if (anim != null) anim.SetBool("IsEating", false);

        // Optional: Keep the bar where it is, or hide it. 
        // If you hide it, the value stays ready for next time.
    }

    void CompleteEating()
    {
        // 1. Reset State
        isEating = false;
        currentBitesDone = totalBitesRequired;

        // 2. UI Feedback
        if (eatingBar != null) eatingBar.value = 1f;
        if (anim != null) anim.SetBool("IsEating", false);
        if (barContainer != null) barContainer.SetActive(false);

        // 3. Unlock Logic (The "Hidden" save)
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        /* If this is Level 1 (Index 1 in Build Settings), 
           nextLevel will be 2. 
        */
        int nextLevel = currentLevelIndex + 1;

        int savedProgress = PlayerPrefs.GetInt("ReachedIndex", 1);

        if (nextLevel > savedProgress)
        {
            PlayerPrefs.SetInt("ReachedIndex", nextLevel);
            PlayerPrefs.Save();
            Debug.Log("Level " + nextLevel + " is now unlocked in the Menu!");
        }

        Debug.Log("Food Finished! You can now exit to the menu to see Level 2 unlocked.");
    }
}