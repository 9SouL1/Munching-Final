using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class FoodManager : MonoBehaviour
{
    public GameObject foodPrefab;
    public Transform[] spawnPoints;
    public int totalFoodLimit = 10;
    
    private int foodsSpawned = 0;
    private GameObject currentFood;

    void Start()
    {
        SpawnNextFood();
    }

    public void SpawnNextFood()
    {
        if (foodsSpawned < totalFoodLimit)
        {
            // Pick a random spawn point
            int randomIndex = Random.Range(0, spawnPoints.Length);
            currentFood = Instantiate(foodPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            foodsSpawned++;
        }
        else
        {
            Debug.Log("Game Over: All food consumed!");
        }
    }

    // Call this from the Player script
    public IEnumerator EatAndRespawnRoutine()
    {
        yield return new WaitForSeconds(2f);
        SpawnNextFood();
    }
}