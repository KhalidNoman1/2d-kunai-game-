using UnityEngine;

// Central place that plays one-shot sound effects.
// Other scripts call SoundManager.Instance.PlayThrow(), etc.
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Drag each clip into its slot")]
    public AudioClip throwSound;
    public AudioClip jumpSound;
    public AudioClip deathSound;
    public AudioClip winSound;

    private AudioSource src;

    void Awake()
    {
        Instance = this;
        src = GetComponent<AudioSource>();
    }

    void Play(AudioClip clip)
    {
        if (clip != null) src.PlayOneShot(clip);
    }

    public void PlayThrow() { Play(throwSound); }
    public void PlayJump()  { Play(jumpSound); }
    public void PlayDeath() { Play(deathSound); }
    public void PlayWin()   { Play(winSound); }
}