using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Main Gameplay Controller
public class GameController : MonoBehaviour
{
    // Enemy Prefab
    public GameObject enemy;
    // Interval between Enemy spawning
    public float spawnIntervalInSeconds = 2f;
    // Internal countdown for the next spawn
    private float nextSpawnCountdown = 2f;
    // Max time playing
    public float maxTimePlayingInSeconds = 30f;

    // Internal map of active enemies points, by instance ID
    private Dictionary<int, int> enemiesPointsById = new Dictionary<int, int>();

    // Register new enemy, so we can manage its lifecycle
    public void RegisterEnemy(int instanceId, int points)
    {
        // Saves the enemy points, indexed by Instance ID
        enemiesPointsById.Add(instanceId, points);
    }

    // Add points to current gameplay
    public void CatchEnemy(int instanceId)
    {
        Debug.Log($"Caught: {instanceId} - Keys: {String.Join(",", enemiesPointsById.Keys)} - Points: {GameState.Score}");
        if (enemiesPointsById.ContainsKey(instanceId))
        {
            Debug.Log($"Points: {GameState.Score}");
            GameState.Score += enemiesPointsById[instanceId];
            enemiesPointsById.Remove(instanceId);
        }
    }

    // Remove reference to Enemy
    public void ReleaseEnemy(int instanceId)
    {
        if (enemiesPointsById.ContainsKey(instanceId))
        {
            enemiesPointsById.Remove(instanceId);
        }
    }

    // Init internal references
    void Start()
    {
        GameState.Score = 0;
        GameState.TimeElapsed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.TimeElapsed >= maxTimePlayingInSeconds)
        {
            enabled = false;
        }
        GameState.TimeElapsed += Time.deltaTime;

        nextSpawnCountdown -= Time.deltaTime;
        if (nextSpawnCountdown <= 0)
        {
            // Instantiate new enemy
            var newEnemy = Instantiate(enemy);

            // Saves the enemy points, indexed by Instance ID
            nextSpawnCountdown = spawnIntervalInSeconds;
        }
    }

    // Render UI
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 50), $"Time Left: {Math.Round(maxTimePlayingInSeconds - GameState.TimeElapsed, 3).ToString()} seconds");
        GUI.Label(new Rect(10, 40, 200, 50), $"Score: {GameState.Score.ToString()}");
    }
}
