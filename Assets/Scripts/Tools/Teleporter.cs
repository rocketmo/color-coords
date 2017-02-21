using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teleports the player between two positions.
/// </summary>
public class Teleporter : Tool {
    /// <summary>
    /// Number of teleporters 
    /// </summary>
    private static int count = 0;

    /// <summary>
    /// The other teleporter.
    /// </summary>
    private Teleporter other;

    /// <summary>
    /// The other teleporter.
    /// </summary>
    public Teleporter Other {
        set {
            other = value;
        }
    }

    /// <summary>
    /// Location of the teleporter on the grid.
    /// </summary>
    private IntVector.IntVector2 location;

    /// <summary>
    /// Location of the teleporter on the grid.
    /// </summary>
    public IntVector.IntVector2 Location {
        get {
            return location;
        }
    }

    /// <summary>
    /// Grid the teleporter is on.
    /// </summary>
    private Grid grid;

    /// <summary>
    /// Pulses the tool's size.
    /// </summary>
    private SizePulser pulse;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    /// <param name="parent">Parent game object.</param>
    /// <param name="o">Connected teleporter.</param>
    public Teleporter(int x, int y, Grid g, GameObject parent, Teleporter o = null) {
        location = new IntVector.IntVector2(x, y);
        grid = g;
        other = o;

        // size pulser settings
        float r = UnityEngine.Random.Range(0.1f, 0.9f);
        Vector2 small = new Vector2(1.0f, 0.95f);
        Vector2 big = new Vector2(1.0f, 1.05f);

        // create the teleporter
        ColourPicker.Colour col = (ColourPicker.Colour)((count / 2) % ColourPicker.ColourCount);
        GameObject teleObject = ResourceLoader.GetSpriteGameObject(col + "Teleporter", parent, (float)x, (float)y, "Tools", 1, "Sprites/Tools/Teleporter", col);
        pulse = teleObject.AddComponent<SizePulser>();
        pulse.SetParams(true, r, small, big);

        // increment count
        count += 1;
    }

    /// <summary>
    /// Teleports the player to the connected teleporter.
    /// </summary>
    /// <param name="p">The player.</param>
    public override void PerformAction(Player p) {
        grid.UpdatePlayerTile();
        if (other != null) {
            grid.TeleportPlayer(other.Location);
            if (grid.GetTileTypeAt(other.Location) == Tile.TileType.Ice) {
                p.MakeExtraMove(p.LastMove);
            }
        }
        
    }

    /// <summary>
    /// Tool does change movement.
    /// </summary>
    /// <returns>Always true.</returns>
    public override bool CanRedirect() {
        return true;
    }

    /// <summary>
    /// Resets the count.
    /// </summary>
    public static void ResetCount() {
        count = 0;
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
