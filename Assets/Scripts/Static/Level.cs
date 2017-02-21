using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level  {
    /// <summary>
    /// Text file containing level information
    /// </summary>
    private static TextAsset levelInfo = Resources.Load("Levels/sandbox") as TextAsset;

    /// <summary>
    /// Text file containing level information
    /// </summary>
    public static TextAsset LevelInfo {
        get {
            return levelInfo;
        }
    }

    /// <summary>
    /// Level number.
    /// </summary>
    private static int levelNum = 0;

    /// <summary>
    /// Highest level in the game.
    /// </summary>
    private const int MAXLEVEL = 18;

    /// <summary>
    /// Gets the level information for the wanted level. Level must be valid.
    /// </summary>
    /// <param name="levelNum">The level number.</param>
    public static void SetLevel(int level) {
        levelNum = level;
        levelInfo = Resources.Load("Levels/level-" + level.ToString()) as TextAsset;
    }

    /// <summary>
    /// Try and load the next level.
    /// </summary>
    public static bool NextLevel() {
        int num = levelNum + 1;
        TextAsset t = Resources.Load("Levels/level-" + num.ToString()) as TextAsset;

        if (t == null) {
            return false;
        }

        levelNum = num;
        levelInfo = t;
        return true;
    }

    /// <summary>
    /// Try and load the previous level.
    /// </summary>
    public static bool PreviousLevel() {
        int num = levelNum - 1;
        TextAsset t = Resources.Load("Levels/level-" + num.ToString()) as TextAsset;

        if (t == null) {
            return false;
        }

        levelNum = num;
        levelInfo = t;
        return true;
    }

    /// <summary>
    /// Checks if the current level is the highest.
    /// </summary>
    /// <returns>Returns true if the current level is the last one; false otherwise.</returns>
    public static bool IsAtMax() {
        return (levelNum == MAXLEVEL);
    }
}
