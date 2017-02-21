using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the player erase instead of colour.
/// </summary>
public class Eraser : Tool {

    /// <summary>
    /// Pulses the tool's size.
    /// </summary>
    private SizePulser pulse;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    /// <param name="parent">Parent of the eraser game object.</param>
    public Eraser (int x, int y, GameObject parent) {
        // size pulser settings
        float r = UnityEngine.Random.Range(0.1f, 0.9f);
        Vector2 small = new Vector2(0.95f, 0.95f);
        Vector2 big = new Vector2(1.05f, 1.05f);

        // create game object
        GameObject eraserObject = ResourceLoader.GetSpriteGameObject("Eraser", parent, (float)x, (float)y, "Tools", 0, "Sprites/Tools/Eraser");
        pulse = eraserObject.AddComponent<SizePulser>();
        pulse.SetParams(false, r, small, big);
    }

    /// <summary>
    /// Turns the player into an eraser.
    /// </summary>
    /// <param name="p">The player.</param>
    public override void PerformAction(Player p) {
        p.BeginErase();
    }

    /// <summary>
    /// Tool cannot change movement.
    /// </summary>
    /// <returns>Always false.</returns>
    public override bool CanRedirect() {
        return false;
    }

    /// <summary>
    /// Pauses the tool animation.
    /// </summary>
    public override void Pause() {
        pulse.Paused = !pulse.Paused;
    }

    /// <summary>
    /// Turns on tool animation.
    /// </summary>
    public override void TurnOnAnimation() {
        pulse.Paused = false;
    }
}
