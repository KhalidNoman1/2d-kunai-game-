using UnityEngine;

// Resets the player to a spawn point when they hit a hazard or fall too far.
public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Where the player reappears after dying. Defaults to their start position.")]
    public Vector2 spawnPoint;
    [Tooltip("If the player falls below this Y height, they die.")]
    public float killHeight = -10f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Remember where we started as the default respawn spot.
        spawnPoint = transform.position;
    }

    void Update()
    {
        // Fell off the bottom of the level.
        if (transform.position.y < killHeight)
            Respawn();
    }

    // Anything tagged "Hazard" that we touch kills us.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
            Respawn();
    }

    void Respawn()
    {
        // Zero out momentum so we don't fly off on respawn, then teleport back.
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint;
    }
}