using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the main menu buttons.
public class MenuManager : MonoBehaviour
{
    // Loads the gameplay scene. Must match your game scene's exact name.
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Quits the game (does nothing in the editor, works in a real build).
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed");
    }
}
