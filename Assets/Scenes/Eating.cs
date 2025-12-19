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

    // CHANGED TO PUBLIC: Now the NPCChaser script can detect this
    public bool isEating = false;

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
        isEating = true; // NPC will start chasing now

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
        isEating = false; // NPC stops chasing because player stopped eating
        if (barContainer != null) barContainer.SetActive(false);
        if (anim != null) anim.SetBool("IsEating", false);
    }

    void CompleteEating()
    {
        // 1. Reset State
        isEating = false; // NPC stops chasing because food is finished
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