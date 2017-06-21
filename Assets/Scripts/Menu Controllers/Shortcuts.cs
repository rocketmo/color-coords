using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles certain button presses.
/// </summary>
public class Shortcuts : MonoBehaviour {

	/// <summary>
    /// Checks for button input.
    /// </summary>
	void Update () {
        if (Input.GetButtonDown("Quit")) {   // close application
            Quit();
        } else if (SceneManager.GetActiveScene().name == "Puzzle") {    // shortcuts during puzzles

            if (Input.GetButtonDown("LevelSelect")) {   // return to level select screen
                LevelSelect();
            } else if (Input.GetButtonDown("Next")) {
                LoadNextLevel();
            } else if (Input.GetButtonDown("Back")) {
                LoadPreviousLevel();
            }
        }
	}

    /// <summary>
    /// Returns to level select screen
    /// </summary>
    public void LevelSelect() {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }

    /// <summary>
    /// Returns to main menu
    /// </summary>
    public void ReturnToMenu() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit() {
        Application.Quit();
    }

    /// <summary>
    /// Try and load the next level
    /// </summary>
    public void LoadNextLevel() {
        if (Level.NextLevel()) {   // change level
            SceneManager.LoadScene("Puzzle", LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Try and load the previous level
    /// </summary>
    public void LoadPreviousLevel() {
        if (Level.PreviousLevel()) {   // change level
            SceneManager.LoadScene("Puzzle", LoadSceneMode.Single);
        }
    }
}
