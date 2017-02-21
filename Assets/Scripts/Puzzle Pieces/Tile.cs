using UnityEngine;
using System.Collections;

/// <summary>
/// Individual tile piece on a puzzle grid.
/// </summary>
public class Tile {

    /// <summary>
    /// Type of tiles on the grid.
    /// Default and Ice can be painted on. Ice will make the player slide.
    /// Dark cannot be painted on. Blank tiles are empty.
    /// </summary>
    public enum TileType { Default, Ice, Dark, Blank };

    /// <summary>
    /// The type of this tile.
    /// </summary>
    private TileType type;

    /// <summary>
    /// Type of this tile
    /// </summary>
    public TileType Type {
        get {
            return type;
        }
    }

    /// <summary>
    /// The tile game object, shows the tile's sprite.
    /// </summary>
    private GameObject tileObject;

    /// <summary>
    /// Colour of the tile.
    /// </summary>
    private ColourPicker.Colour colour;

    /// <summary>
    /// Colour of the tile.
    /// </summary>
    public ColourPicker.Colour Colour {
        get {
            return colour;
        }

        set {
            if (tileObject != null) {
                colour = value;
                SpriteRenderer sprite = tileObject.GetComponent<SpriteRenderer>();

                if (type == TileType.Ice) {
                    sprite.color = Grid.IceColourMap[colour];
                } else {
                    sprite.color = Grid.ColourMap[colour];
                }
            }
        }
    }

    /// <summary>
    /// Default constructor, just creates a blank tile.
    /// </summary>
    public Tile() {
        type = TileType.Blank;
        colour = ColourPicker.Colour.White;
        tileObject = null;
    }

    /// <summary>
    /// Constructor, creates a tile of the given type.
    /// </summary>
    /// <param name="t">type of tile</param>
    /// <param name="g">grid</param>
    /// <param name="x">x position of tile</param>
    /// <param name="y">y position of tile</param>
    public Tile(TileType t, GameObject g, int x, int y) {
        type = t;
        colour = ColourPicker.Colour.White;

        if (t == TileType.Blank) {  // no game object necessary if tile is blank
            tileObject = null;
        } else { // create appropriate game object for other tiles
            int checker = (x + y) % 2;
            if (t == TileType.Default) {
                tileObject = ResourceLoader.GetSpriteGameObject(t + "Tile", g, (float)x, (float)y, "Tiles", 1, "Sprites/Tiles/Tile-Default-" + checker.ToString());
            } else if (t == TileType.Ice) {
                tileObject = ResourceLoader.GetSpriteGameObject(t + "Tile", g, (float)x, (float)y, "Tiles", 1, "Sprites/Tiles/Tile-Ice-" + checker.ToString());
                ResourceLoader.GetSpriteGameObject(t + "TileGlint", tileObject, 0.0f, 0.0f, "Tiles", 3, "Sprites/Tiles/Tile-Ice-Glint");
            } else {
                tileObject = ResourceLoader.GetSpriteGameObject(t + "Tile", g, (float)x, (float)y, "Tiles", 1, "Sprites/Tiles/Tile-Dark-" + checker.ToString());
            } 

            // create outline
            ResourceLoader.GetSpriteGameObject(t + "TileOutline", tileObject, 0.0f, 0.0f, "Tiles", 0, "Sprites/Tiles/Tile-Outline");
        }
    }

    /// <summary>
    /// Constructor, creates a solution tile.
    /// </summary>
    /// <param name="col"></param>
    public Tile(ColourPicker.Colour col, GameObject g, int x, int y, TileType tt) {
        type = TileType.Default;
        colour = col;

        if (tt == TileType.Blank) {  // no game object necessary if tile is blank or a block
            tileObject = null;
        } else { // create appropriate game object for other colours
            int checker = (x + y) % 2;
            if (tt == TileType.Default) {
                tileObject = ResourceLoader.GetSpriteGameObject(col + "Tile", g, (float)x, (float)y, "Tiles", 2, "Sprites/Tiles/Tile-Default-" + checker.ToString(), col);
            } else if (tt == TileType.Ice) {
                tileObject = ResourceLoader.GetSpriteGameObject(col + "Tile", g, (float)x, (float)y, "Tiles", 2, "Sprites/Tiles/Tile-Ice-" + checker.ToString());
                tileObject.GetComponent<SpriteRenderer>().color = Grid.IceColourMap[col];
            } else {
                tileObject = ResourceLoader.GetSpriteGameObject(col + "Tile", g, (float)x, (float)y, "Tiles", 2, "Sprites/Tiles/Tile-Dark-" + checker.ToString());
            }
        }
    }
}
