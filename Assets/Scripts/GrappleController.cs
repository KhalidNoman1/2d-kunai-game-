using UnityEngine;

// Core grapple-swing movement for Kunai.
// Controls: Left-click = grapple, E = reel in + launch up, Space = once-per-swing jump.
// Attach to the player GameObject. Requires a Rigidbody2D (Dynamic) and a LineRenderer.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class GrappleController : MonoBehaviour
{
    [Header("Grapple")]
    [Tooltip("Max distance the kunai can reach to find an anchor.")]
    public float maxGrappleDistance = 12f;
    [Tooltip("Which layers count as grappable surfaces.")]
    public LayerMask grappleLayer;
    [Tooltip("How fast you reel in (hold E) toward the anchor.")]
    public float reelSpeed = 6f;
    [Tooltip("Extra upward push while holding E — gives reeling a launch feel.")]
    public float reelLaunchForce = 12f;
    [Tooltip("Shortest the rope can reel to, so you don't slam into the anchor.")]
    public float minRopeLength = 0.5f;
    [Tooltip("Velocity multiplier applied the moment you release — makes letting go feel like a launch.")]
    public float releaseBoost = 1.3f;

    [Header("Jump")]
    [Tooltip("Upward pop of the once-per-swing jump.")]
    public float jumpForce = 8f;

    [Header("Air control (optional feel tuning)")]
    [Tooltip("Small sideways nudge while swinging, for finer control.")]
    public float airControlForce = 2f;

    private Rigidbody2D rb;
    private LineRenderer rope;
    private Camera cam;

    private bool isGrappling;
    private Vector2 anchorPoint;   // world point the kunai latched onto
    private float ropeLength;      // current max distance from anchor
    private bool canJump;          // one jump per grapple; refills on each new latch

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rope = GetComponent<LineRenderer>();
        cam = Camera.main;

        rope.positionCount = 2;
        rope.enabled = false;
    }

    void Update()
    {
        // Fire / release the grapple.
        if (Input.GetMouseButtonDown(0)) TryGrapple();
        if (Input.GetMouseButtonUp(0)) ReleaseGrapple();

        // One small jump per swing: only while grappling and if it hasn't been used yet.
        if (Input.GetKeyDown(KeyCode.Space) && isGrappling && canJump)
            Jump();

        if (isGrappling) DrawRope();
    }

    void FixedUpdate()
    {
        if (!isGrappling) return;

        // Hold E to reel in AND get an upward launch while doing it.
        if (Input.GetKey(KeyCode.E))
        {
            ropeLength = Mathf.Max(minRopeLength, ropeLength - reelSpeed * Time.fixedDeltaTime);
            rb.AddForce(Vector2.up * reelLaunchForce, ForceMode2D.Force);
        }

        // Small left/right nudge for finer swing control.
        float h = Input.GetAxisRaw("Horizontal");
        if (h != 0f) rb.AddForce(new Vector2(h * airControlForce, 0f), ForceMode2D.Force);

        ApplyRopeConstraint();
    }

    // Shoot toward the mouse; latch onto the first grappable thing in range.
    // Shoot toward the mouse; latch onto the first grappable thing in range.
    void TryGrapple()
    {
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 origin = rb.position;
        Vector2 dir = (mouseWorld - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxGrappleDistance, grappleLayer);
        if (hit.collider != null)
        {
            isGrappling = true;
            if (GameManager.Instance != null) GameManager.Instance.StartTimer(); // start timer on first grapple
            anchorPoint = hit.point;
            ropeLength = Vector2.Distance(origin, anchorPoint);
            rope.enabled = true;
            canJump = true;
        }
    }

    // Detach and pop upward, keeping swing momentum. Consumes the one swing-jump.
    void Jump()
    {
        canJump = false;
        isGrappling = false;
        rope.enabled = false;
        // Keep horizontal swing speed, replace vertical with the jump pop.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void ReleaseGrapple()
    {
        isGrappling = false;
        rope.enabled = false;
        // Fling off: amplify the momentum you built up during the swing.
        rb.linearVelocity *= releaseBoost;
    }

    // Keeps the player on (or inside) a circle of radius ropeLength around the anchor.
    // This is what turns gravity into a swing instead of a fall.
    void ApplyRopeConstraint()
    {
        Vector2 pos = rb.position;
        Vector2 toAnchor = anchorPoint - pos;
        float dist = toAnchor.magnitude;

        if (dist > ropeLength)
        {
            Vector2 dir = toAnchor / dist;                 // unit vector toward anchor
            rb.position = anchorPoint - dir * ropeLength;  // snap back onto the rope circle

            // Remove the part of velocity pulling away from the anchor,
            // keep the sideways part so the swing is preserved.
            float outwardVel = Vector2.Dot(rb.linearVelocity, -dir);
            if (outwardVel > 0f)
                rb.linearVelocity += dir * outwardVel;
        }
    }

    void DrawRope()
    {
        rope.SetPosition(0, transform.position); // player's hand
        rope.SetPosition(1, anchorPoint);        // kunai stuck in the wall
    }
}