using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tools are used by the player to complete puzzles.
/// </summary>
abstract public class Tool {
    /// <summary>
    /// Performs the tool's action when the player hits the tool.
    /// </summary>
    /// <param name="p">The player.</param>
    abstract public void PerformAction(Player p);

    /// <summary>
    /// Does the tool change the player's position?
    /// </summary>
    /// <returns>Returns true if the tool can change the player's position, false otherwise.</returns>
    abstract public bool CanRedirect();

    /// <summary>
    /// Pauses/unpauses tool animation.
    /// </summary>
    abstract public void Pause();

    /// <summary>
    /// Always turns on animation.
    /// </summary>
    abstract public void TurnOnAnimation();
}
