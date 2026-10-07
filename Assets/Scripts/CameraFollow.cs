using UnityEngine;

// Smoothly follows a target (the player) so the camera trails you as you climb.
public class CameraFollow : MonoBehaviour
{
    public Transform target;          // drag the Player here in the Inspector
    public float smoothTime = 0.2f;   // lower = snappier, higher = floatier
    public Vector3 offset = new Vector3(0f, 0f, -10f); // keep z = -10 so the 2D camera sees the scene

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
    }
}