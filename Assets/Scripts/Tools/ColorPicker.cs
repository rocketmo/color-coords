using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ColourPicker tool is used to change the colour of the player.
/// When the player moves onto a ColourPicker, their colour changes, allowing them to colour tiles that colour. 
/// </summary>
public class ColourPicker : Tool {

    /// <summary>
    /// Possible colors for the color pickers, tiles and player. White is default colour.
    /// </summary>
    public enum Colour { Red, Blue, Yellow, Orange, Purple, Green, White };

    /// <summary>
    /// Number of colours.
    /// </summary>
    private static int colourCount = Enum.GetNames(typeof(ColourPicker.Colour)).Length;

    /// <summary>
    /// Number of colours.
    /// </summary>
    public static int ColourCount {
        get {
            return colourCount;
        }
    }

    /// <summary>
    /// Colour of the colour picker.
    /// </summary>
    private Colour colour;

    /// <summary>
    /// Rotates the tool.
    /// </summary>
    private AutoRotator rotate;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="x">x coordinate on the grid.</param>
    /// <param name="y">y coordinate on the grid.</param>
    /// <param name="col">Colour of the colour picker.</param>
    /// <param name="g">Grid that the colour picker is on.</param>
    /// <param name="parent">Parent of the base object.</param>
    public ColourPicker(int x, int y, Colour col, GameObject parent) {
        colour = col;

        // create the colour picker
        GameObject cpObject = ResourceLoader.GetSpriteGameObject(colour + "ColourPicker", parent, (float)x, (float)y, "Tools", 0, "Sprites/Tools/ColorPicker", col);
        rotate = cpObject.AddComponent<AutoRotator>();
    }

    /// <summary>
    /// Colours the player and the tile that the colour picker is on (if it is possible).
    /// </summary>
    /// <param name="p">Player that is activating the colour picker.</param>
    public override void PerformAction(Player p) {
        p.Colour = colour;
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
