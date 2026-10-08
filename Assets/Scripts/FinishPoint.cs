using UnityEngine;

// Put this on the top platform (solid collider). Wins only when the player
// lands on TOP of it, not when they bonk the side or underside.
public class FinishPoint : MonoBehaviour
{
    private bool finished = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (finished) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // Check the contact came from above: the collision normal points downward
        // into the platform, meaning the player is on top.
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)   // player landed on top
            {
                finished = true;
                if (GameManager.Instance != null) GameManager.Instance.WinLevel();
                return;
            }
        }
    }
}