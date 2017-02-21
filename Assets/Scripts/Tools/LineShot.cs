using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoots colour in certain directions.
/// </summary>
public class LineShot : Tool {
    /// <summary>
    /// Colour of this tool.
    /// </summary>
    private ColourPicker.Colour colour;

    /// <summary>
    /// Grid that this tool is on.
    /// </summary>
    private Grid grid;

    /// <summary>
    /// Location of the line-shot on the grid.
    /// </summary>
    private IntVector.IntVector2 location;

    /// <summary>
    /// Directions to shoot in.
    /// </summary>
    private List<Grid.Direction> directions;

    /// <summary>
    /// Pulses the tool's size.
    /// </summary>
    private SizePulser pulse;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    /// <param name="g">Grid that the line-shot is on.</param>
    /// <param name="col">Colour of the line-shot.</param>
    /// <param name="dir">List of directions the line-shot will shoot at.</param>
    /// <param name="parent">Game object parent of the line-shot.</param>
    public LineShot(int x, int y, Grid g, ColourPicker.Colour col, List<Grid.Direction> dir, GameObject parent) {
        location = new IntVector.IntVector2(x, y);
        grid = g;
        colour = col;
        directions = dir;

        // size pulser settings
        float r = UnityEngine.Random.Range(0.1f, 0.9f);
        Vector2 small = new Vector2(0.95f, 0.95f);
        Vector2 big = new Vector2(1.05f, 1.05f);

        // create the game objects
        GameObject mainObject = new GameObject();
        mainObject.name = col + "LineShot";
        mainObject.transform.parent = parent.transform;
        Vector3 mainPos = mainObject.transform.localPosition;
        mainPos.x = (float)x;
        mainPos.y = (float)y;
        mainObject.transform.localPosition = mainPos;
        pulse = mainObject.AddComponent<SizePulser>();
        pulse.SetParams(true, r, small, big);

        // line-shot base
        ResourceLoader.GetSpriteGameObject("LineShotBase", mainObject, 0.0f, 0.0f, "Tools", 0, "Sprites/Tools/LineShot", col);

        // arrows
        foreach (Grid.Direction d in dir) {
            GameObject arrow = ResourceLoader.GetSpriteGameObject(d + "LineShotArrow", mainObject, 0.0f, 0.0f, "Tools", 0, "Sprites/Tools/LineShot-Arrow", col);

            if (d == Grid.Direction.Left) {
                arrow.transform.Rotate(Vector3.forward, 90.0f);
            } else if (d == Grid.Direction.Right) {
                arrow.transform.Rotate(Vector3.forward, -90.0f);
            } else if (d == Grid.Direction.Down) {
                arrow.transform.Rotate(Vector3.forward, 180.0f);
            }
        }
    }

    /// <summary>
    /// Shoots colour in the appropriate directions.
    /// </summary>
    /// <param name="p"></param>
    public override void PerformAction(Player p) {
        p.Colour = colour;
        foreach (Grid.Direction d in directions) {
            grid.ColourLineFrom(location, d, colour);
        }
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
