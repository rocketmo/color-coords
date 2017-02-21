using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the direction of the player.
/// </summary>
public class Redirection : Tool {
    /// <summary>
    /// Direction to redirect in.
    /// </summary>
    private Grid.Direction direction;

    /// <summary>
    /// Pulses the tool's size.
    /// </summary>
    private SizePulser pulse;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    /// <param name="parent">Parent object</param>
    /// <param name="dir">Direction to move in.</param>
    public Redirection(int x, int y, GameObject parent, Grid.Direction dir) {
        direction = dir;

        // size pulser settings
        float r = UnityEngine.Random.Range(0.1f, 0.9f);
        Vector2 small = new Vector2(0.95f, 0.95f);
        Vector2 big = new Vector2(1.05f, 1.05f);

        // make Redirect
        GameObject rObject = ResourceLoader.GetSpriteGameObject(dir + "Redirect", parent, (float)x, (float)y, "Tools", 1, "Sprites/Tools/Redirect");
        pulse = rObject.AddComponent<SizePulser>();
        pulse.SetParams(true, r, small, big);

        // rotate sprite (if necessary)
        if (dir == Grid.Direction.Left) {
            rObject.transform.Rotate(Vector3.forward, 90.0f);
        } else if (dir == Grid.Direction.Right) {
            rObject.transform.Rotate(Vector3.forward, -90.0f);
        } else if (dir == Grid.Direction.Down) {
            rObject.transform.Rotate(Vector3.forward, 180.0f);
        }
    }

    /// <summary>
    /// Redirects the player's movement.
    /// </summary>
    /// <param name="p">The player.</param>
    public override void PerformAction(Player p) {
        p.MakeExtraMove(direction);
    }

    /// <summary>
    /// Tool does change movement.
    /// </summary>
    /// <returns>Always true.</returns>
    public override bool CanRedirect() {
        return true;
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
