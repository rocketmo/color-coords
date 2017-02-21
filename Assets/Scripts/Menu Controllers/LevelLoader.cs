using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads levels.
/// </summary>
public class LevelLoader : MonoBehaviour {
    /// <summary>
    /// Loads the given level and changes the scene.
    /// </summary>
    /// <param name="levelNum">The level number.</param>
    public void LoadLevel(int levelNum) {
        Level.SetLevel(levelNum);
        SceneManager.LoadScene("Puzzle", LoadSceneMode.Single);
    }
}
