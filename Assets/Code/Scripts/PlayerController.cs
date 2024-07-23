using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls Player behavior
public class PlayerController : MonoBehaviour
{
    // Holds reference to the game controller
    public GameObject gameControllerObject;
    // Holds reference to the game controller script
    private GameController gameController;
    // Vector pointing to the movement direction, based on the input
    private Vector2 movementDir;
    // Constant to control the movement speed
    public float moveSpeed = 5f;
    // Constant to control the min altitude of the hook
    public float minAltitude = -8.0f;
    // Constant to control the max altitude of the hook
    public float maxAltitude = 8.0f;


    // Action to move up
    private void OnMoveUp()
    {
        movementDir = Vector2.up;
    }

    // Action to move down
    private void OnMoveDown()
    {
        movementDir = Vector2.down;
    }

    // Init internal references
    void Start()
    {
        gameController = gameControllerObject.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        var currPosition = transform.position.y;
        if (
            (movementDir == Vector2.down && currPosition > minAltitude) || // Going DOWN, should stop at the bottom
            (movementDir == Vector2.up && currPosition < maxAltitude) // Going UP, should stop at the top
        )
        {
            transform.Translate(moveSpeed * Time.deltaTime * movementDir);
        }
    }

    // Check collision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            gameController.CatchEnemy(other.transform.parent.gameObject.GetInstanceID());
        }
    }
}
