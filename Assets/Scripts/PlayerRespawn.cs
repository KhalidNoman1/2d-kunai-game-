using UnityEngine;

// Resets the player to a fixed start point when they hit a hazard or fall.
public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Drag the StartPoint object here — the player always respawns here.")]
    public Transform startPoint;
    [Tooltip("If the player falls below this Y height, they die.")]
    public float killHeight = -10f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.y < killHeight)
            Respawn();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
            Respawn();
    }

    void Respawn()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayDeath();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        // Always teleport to the fixed start marker.
        if (startPoint != null)
            transform.position = startPoint.position;
    }
}