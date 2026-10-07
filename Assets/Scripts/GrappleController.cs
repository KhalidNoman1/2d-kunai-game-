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

        // Hold E to reel in AND get an upward launch