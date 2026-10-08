using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Runs the run-timer, shows best time, and handles the win screen.
// One of these in the scene; other scripts call its public methods.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;   // simple global access

    [Header("UI references (drag these in)")]
    public Text timerText;       // live timer, top of screen
    public GameObject winPanel;  // the "You Win" panel (hidden until you win)
    public Text winTimeText;     // "Your time: 12.3s" on the panel
    public Text bestTimeText;    // "Best: 10.1s" on the panel

    private float currentTime;
    private bool timing = false;
    private bool finished = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false); // hide win screen at start
    }

    void Update()
    {
        if (timing && !finished)
        {
            currentTime += Time.deltaTime;
            if (timerText != null) timerText.text = currentTime.ToString("F1") + "s";
        }
    }

    // Called by the grapple script the first time you fire the kunai.
    public void StartTimer()
    {
        if (!timing) timing = true;
    }

    // Called by the FinishPoint when you reach the top.
    public void WinLevel()
    {
        if (finished) return;
        finished = true;
        timing = false;

        // Save best time with PlayerPrefs (persists between runs).
        float best = PlayerPrefs.GetFloat("BestTime", 0f);
        if (best == 0f || currentTime < best)
        {
            best = currentTime;
            PlayerPrefs.SetFloat("BestTime", best);
        }

        if (winPanel != null) winPanel.SetActive(true);
        if (winTimeText != null) winTimeText.text = "Your time: " + currentTime.ToString("F1") + "s";
        if (bestTimeText != null) bestTimeText.text = "Best: " + best.ToString("F1") + "s";
    }

    // Hooked to a "Retry" button on the win panel.
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}