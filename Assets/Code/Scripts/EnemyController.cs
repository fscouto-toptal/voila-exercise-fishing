using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Schema;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Possible options of enemies to instantiate
[System.Serializable]
public struct EnemyOption
{
    public GameObject sprite;
    public int probability;
    public int points;
}

// Controller for Enemy behavior
public class EnemyController : MonoBehaviour
{
    // List of all types of enemies available
    public EnemyOption[] enemies;
    // Holds reference to the game controller
    public GameObject gameControllerObject;
    // Holds reference to the game controller script
    private GameController gameController;
    // Holds reference to the sprite animator
    private Animator animator;

    // Probability of not spawning
    public float noSpawnProb = 0.2f;
    // Left spawning limit
    public float leftSpawnLimit = -22f;
    // Right spawning limit
    public float rightSpawnLimit = 22f;
    // Top spawning limit
    public float topSpawnLimit = -8f;
    // Bottom spawning limit
    public float bottomSpawnLimit = 8f;

    // Direction of the movement
    private Vector2 movementDir;
    // Constant to control the movement speed
    public float moveSpeed = 8f;

    // How many points the player earns if this enemy is caught
    private int enemyPoints;

    // Init internal references
    void Start()
    {
        gameController = gameControllerObject.GetComponent<GameController>();

        var enemyOption = PickRandomEnemy();
        var sprite = enemyOption.sprite;

        if (sprite != null)
        {
            // Set points for current enemy
            enemyPoints = enemyOption.points;

            // Create a new instance of the selected sprite, as a child of this game object
            Instantiate(sprite, gameObject.transform, worldPositionStays:false);

            // Assign the animator for the selected sprite
            animator = sprite.GetComponent<Animator>();

            // Get spawning height
            var spawningHeight = GetSpawningHeight();

            // Set spawning position
            SetSpawningPosition(spawningHeight);

            gameController.RegisterEnemy(gameObject.GetInstanceID(), enemyPoints);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime * Vector2.left);

        var currPosition = transform.position.x;
        if (
            (movementDir == Vector2.left && currPosition < leftSpawnLimit) || // Going LEFT, should stop at left limits
            (movementDir == Vector2.right && currPosition > rightSpawnLimit) // Going RIGHT, should stop at the right limits
        )
        {
            // Release the object
            DestroyMe();
        }
    }

    // Randomly set the final spawning position
    void SetSpawningPosition(float height)
    {
        // Radomly chooses the movement direction (left or right)
        if (UnityEngine.Random.value <= 0.5f)
        {
            transform.position = new Vector2(rightSpawnLimit, height);
            movementDir = Vector2.left;
        }
        else
        {
            transform.position = new Vector2(leftSpawnLimit, height);
            movementDir = Vector2.right;
            // Rotate 180 degrees in the Y axis, making the NPC move to the right
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    // Randomly pick the height to spawn
    float GetSpawningHeight()
    {
        return UnityEngine.Random.Range(topSpawnLimit, bottomSpawnLimit);
    }

    // Randomly pick the sprite
    EnemyOption PickRandomEnemy()
    {
        // Ordered array of instantiable enemies
        var options = enemies.ToList();
        // Ordered array of sprites probabilities
        var probs = enemies.Select(e => e.probability).ToList();

        // Total probability, increased by the "no spawning" chances
        var total = (float)probs.Sum() / ((float)1 - noSpawnProb);
        // Randomized number
        var chosen = UnityEngine.Random.value;

        // Accumulated probability for the random logic
        var accumulated = 0f;
        for (var i = 0; i < probs.Count; i++) {
            accumulated += (float)probs[i] / total;
            if (chosen <= accumulated) {
                return options[i];
            }
        }
        
        // Remove the enemy because there is no sprite selected
        DestroyMe();
        return new EnemyOption();
    }

    // Destroy this Enemy
    void DestroyMe()
    {
        gameController.ReleaseEnemy(GetInstanceID());
        Destroy(gameObject);
    }
}
