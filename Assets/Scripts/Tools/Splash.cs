using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Colours the grid within its radius.
/// </summary>
public class Splash : Tool {

    /// <summary>
    /// Splash colour.
    /// </summary>
    private ColourPicker.Colour colour;

    /// <summary>
    /// Grid that the splash is on.
    /// </summary>
    private Grid grid;

    /// <summary>
    /// Location of the splash on the grid.
    /// </summary>
	private IntVector.IntVector2 location;

    /// <summary>
    /// The radius of the splash.
    /// </summary>
    private int splashRadius;

    /// <summary>
    /// Rotates the tool.
    /// </summary>
    private AutoRotator rotate;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    /// <param name="col">Colour of the splash.</param>
    /// <param name="g">Grid the splash is on.</param>
    /// <param name="parent">Parent of the base object.</param>
    public Splash(int x, int y, ColourPicker.Colour col, Grid g, GameObject parent, int radius = 2) {
        location = new IntVector.IntVector2(x, y);
        colour = col;
        grid = g;
        splashRadius = radius;

        // create the game object
        GameObject splashObject = ResourceLoader.GetSpriteGameObject(colour + "Splash", parent, (float)x, (float)y, "Tools", 0, "Sprites/Tools/Splash", col);
        rotate = splashObject.AddComponent<AutoRotator>();
    }

    /// <summary>
    /// Colours the player and the surrounding tiles within the splash radius.
    /// </summary>
    /// <param name="p"></param>
    public override void PerformAction(Player p) {
        p.Colour = colour;
        grid.Splash(location, splashRadius, colour);
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
        rotate.Paused = !rotate.Paused;
    }

    /// <summary>
    /// Turns on tool animation.
    /// </summary>
    public override void TurnOnAnimation() {
        rotate.Paused = false;
    }
}
