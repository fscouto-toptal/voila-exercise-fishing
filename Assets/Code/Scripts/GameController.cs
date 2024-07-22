using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main Gameplay Controller
public class GameController : MonoBehaviour
{
    public GameObject enemy;
    // Interval between Enemy spawning
    public float spawnIntervalInSeconds = 2f;
    // Internal countdown for the next spawn
    private float nextSpawnCountdown = 2f;

    // Update is called once per frame
    void Update()
    {
        nextSpawnCountdown -= Time.deltaTime;
        if (nextSpawnCountdown <= 0)
        {
            Instantiate(enemy);
            nextSpawnCountdown = spawnIntervalInSeconds;
        }
    }
}
