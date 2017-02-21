using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles puzzle grid behaviour.
/// </summary>
public class Grid : MonoBehaviour {

    /// <summary>
    /// Possible directions the player can move in
    /// </summary>
    public enum Direction { Left, Right, Up, Down };

    #region Class members

    /// <summary>
    /// Maps color picker values to RGB colors.
    /// </summary>
    public static Dictionary<ColourPicker.Colour, Color> ColourMap = CreateColourMap();

    /// <summary>
    /// Maps color picker values to RGB colors for ice tiles.
    /// </summary>
    public static Dictionary<ColourPicker.Colour, Color> IceColourMap = CreateIceColourMap();

    /// <summary>
    /// Counter for the number of moves used.
    /// </summary>
    public MoveCounter moveCount;

    /// <summary>
    /// Pause menu.
    /// </summary>
    public GameObject pauseMenu;

    /// <summary>
    /// Maps grid positions to tools at those locations.
    /// </summary>
    private Dictionary<IntVector.IntVector2, Tool> gridTools;

    /// <summary>
    /// Player controlled object.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Mini-guide cursor.
    /// </summary>
    private GameObject cursor;

    /// <summary>
    /// Camera for the level.
    /// </summary>
    public GameObject cam;

    /// <summary>
    /// Message that displays when the puzzle is complete.
    /// </summary>
    public WinMessage winMessage;

    /// <summary>
    /// Game object that parents all tiles and tools on the grid.
    /// </summary>
    private GameObject gridObjects;

    /// <summary>
    /// Game object that parents all tiles of the puzzle solution.
    /// </summary>
    private GameObject solutionGrid;

    /// <summary>
    /// Starting location of the player on the grid.
    /// </summary>
    private IntVector.IntVector2 playerStartLocation;

    /// <summary>
    /// Current location of the player on the grid.
    /// </summary>
    private IntVector.IntVector2 playerCurrentLocation;

    /// <summary>
    /// 2D array of grid tiles. Represents the puzzle grid.
    /// </summary>
    private Tile[][] gridTiles;

    /// <summary>
    /// 2D array of grid tiles. Represents the puzzle solution.
    /// </summary>
    private Tile[][] solutionTiles;

    /// <summary>
    /// Width of the grid.
    /// </summary>
    private int width;

    /// <summary>
    /// Height of the grid.
    /// </summary>
    private int height;

    /// <summary>
    /// Number of tiles with the right colour.
    /// </summary>
    private int correctTiles;

    /// <summary>
    /// Number of tiles with the correct colour at the start of the puzzle.
    /// </summary>
    private int defaultCorrectTiles;

    /// <summary>
    /// Is the puzzle finished?
    /// </summary>
    private bool puzzleFinished;

    #endregion

    /// <summary>
    /// Initializes and populates the grid.
    /// </summary>
    void Start () {
        // Initial settings
        puzzleFinished = false;
        winMessage.Hide();
        Teleporter.ResetCount();
        pauseMenu.SetActive(false);
        player.GetComponent<Player>().CanMove = true;

        // get level info
        TextAsset levelInfo = Level.LevelInfo;
        string[] txtLines = levelInfo.text.Split('\n');

        // read in level info
        try {
            width = int.Parse(txtLines[0]);
            height = int.Parse(txtLines[1]);

            // create the puzzle grid and solution grid
            gridTiles = new Tile[width][];
            solutionTiles = new Tile[width][];
            for (int i = 0; i < width; i += 1) {
                gridTiles[i] = new Tile[height];
                solutionTiles[i] = new Tile[height];
            }

            // create grid game object
            gridObjects = new GameObject();
            gridObjects.transform.parent = gameObject.transform;
            gridObjects.name = "GridObjects";

            // create grid tiles, set default tile colours
            for (int i = 0; i < width; i += 1) {
                for (int j = 0; j < height; j += 1) {
                    gridTiles[i][height - 1 - j] = new Tile(ReadTileType(txtLines[j + 2][i]), gridObjects, i, height - 1 - j);
                }
            }

            // place player at start location
            player.transform.parent = gridObjects.transform;
            playerStartLocation.x = playerCurrentLocation.x = int.Parse(txtLines[2 + height]);
            playerStartLocation.y = playerCurrentLocation.y = int.Parse(txtLines[3 + height]);
            player.transform.localPosition = new Vector2((float)playerStartLocation.x, (float)playerStartLocation.y);

            // read in tools
            #region Read in tools and create them
            int toolNum = int.Parse(txtLines[4 + height]);
            int lineNum = 5 + height;

            // create tools
            gridTools = new Dictionary<IntVector.IntVector2, Tool>();
            for (int i = 0; i < toolNum; i += 1) {
                char toolType = txtLines[lineNum][0];

                if (toolType == 'c') { // colour picker
                    ColourPicker.Colour col = ReadColour(txtLines[lineNum][1]);
                    IntVector.IntVector2 loc = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 1]), int.Parse(txtLines[lineNum + 2]));
                    ColourPicker cp = new ColourPicker(loc.x, loc.y, col, gridObjects);
                    gridTools.Add(loc, cp);
                    lineNum += 3;
                } else if (toolType == 's') {   // splash
                    ColourPicker.Colour col = ReadColour(txtLines[lineNum][1]);
                    IntVector.IntVector2 loc = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 1]), int.Parse(txtLines[lineNum + 2]));
                    Splash s = new Splash(loc.x, loc.y, col, this, gridObjects);
                    gridTools.Add(loc, s);
                    lineNum += 3;
                } else if (toolType == 'l') {   // line shot
                    ColourPicker.Colour col = ReadColour(txtLines[lineNum][1]);
                    int numDir = int.Parse(txtLines[lineNum + 1]);
                    List<Direction> dir = new List<Direction>();
                    for (int j = 0; j < numDir; j += 1) {
                        dir.Add(ReadDirection(txtLines[lineNum + 2][j]));
                    }
                    IntVector.IntVector2 loc = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 3]), int.Parse(txtLines[lineNum + 4]));
                    LineShot l = new LineShot(loc.x, loc.y, this, col, dir, gridObjects);
                    gridTools.Add(loc, l);
                    lineNum += 5;
                } else if (toolType == 'e') {
                    IntVector.IntVector2 loc = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 1]), int.Parse(txtLines[lineNum + 2]));
                    Eraser e = new Eraser(loc.x, loc.y, gridObjects);
                    gridTools.Add(loc, e);
                    lineNum += 3;
                } else if (toolType == 't') {
                    IntVector.IntVector2 loc1 = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 1]), int.Parse(txtLines[lineNum + 2]));
                    IntVector.IntVector2 loc2 = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 3]), int.Parse(txtLines[lineNum + 4]));
                    Teleporter t1 = new Teleporter(loc1.x, loc1.y, this, gridObjects);
                    Teleporter t2 = new Teleporter(loc2.x, loc2.y, this, gridObjects, t1);
                    t1.Other = t2;
                    gridTools.Add(loc1, t1);
                    gridTools.Add(loc2, t2);
                    lineNum += 5;
                } else if (toolType == 'r') {
                    Direction d = ReadDirection(txtLines[lineNum][1]);
                    IntVector.IntVector2 loc = new IntVector.IntVector2(int.Parse(txtLines[lineNum + 1]), int.Parse(txtLines[lineNum + 2]));
                    Redirection r = new Redirection(loc.x, loc.y, gridObjects, d);
                    gridTools.Add(loc, r);
                    lineNum += 3;
                } else {
                    throw new System.Exception();
                }
            }
            #endregion

            // create solution grid object and the mini-guide
            #region Read in solution and create overlay
            solutionGrid = new GameObject();
            solutionGrid.name = "Solution";
            solutionGrid.transform.parent = transform;

            // create mini-guide
            GameObject miniGuide = new GameObject();
            miniGuide.name = "Mini-Guide";

            GameObject guideOutline = new GameObject();
            guideOutline.name = "GuideOutline";
            guideOutline.transform.localPosition = new Vector2(0.0f, 0.0f);
            guideOutline.transform.parent = miniGuide.transform;

            GameObject guideTiles = new GameObject();
            guideTiles.name = "GuideTiles";
            guideTiles.transform.localPosition = new Vector2(0.0f, 0.0f);
            guideTiles.transform.parent = miniGuide.transform;

            // read in solution
            for (int i = 0; i < width; i += 1) {
                for (int j = 0; j < height; j += 1) {
                    ColourPicker.Colour col = ReadColour(txtLines[lineNum + j][i]);
                    int x = (i + height - 1 - j) % 2;

                    // overlay
                    solutionTiles[i][height - 1 - j] = new Tile(col, solutionGrid, i, height - 1 - j, gridTiles[i][height - 1 - j].Type);

                    // mini guide
                    Tile.TileType t = gridTiles[i][height - 1 - j].Type;
                    if (t != Tile.TileType.Blank) {
                        if (t == Tile.TileType.Default || t == Tile.TileType.Ice) {
                            ResourceLoader.GetSpriteGameObject("GuideTile", guideTiles, i, height - 1 - j, "Guide", 1, "Sprites/Tiles/Tile-Default-" + x.ToString(), col);
                        } else {
                            ResourceLoader.GetSpriteGameObject("GuideTile", guideTiles, i, height - 1 - j, "Guide", 1, "Sprites/Tiles/Tile-Dark-" + x.ToString(), col);
                        } 
                        ResourceLoader.GetSpriteGameObject("GuideOutline", guideOutline, i, height - 1 - j, "Guide", 0, "Sprites/Tiles/Tile-Outline");
                    }
                }
            }

            // set cursor
            cursor = ResourceLoader.GetSpriteGameObject("Cursor", miniGuide, playerCurrentLocation.x, playerCurrentLocation.y, "Guide", 2, "Sprites/Tiles/Cursor");
            solutionGrid.SetActive(false);

            // set position and scale
            miniGuide.transform.position = new Vector2(transform.position.x + ((float)width) + 1.0f, ((float)height) / 4);
            miniGuide.transform.localScale = new Vector2(0.35f, 0.35f);
            #endregion

            // center the camera on the grid
            float aspectMultiplier = (16.0f / 9.0f) / ((float)Screen.width / Screen.height);
            cam.transform.position = new Vector3(((float)width) * 1.35f / 2.0f, ((float)height - 1) / 2.0f, -10.0f);
            cam.GetComponent<Camera>().orthographicSize = Mathf.Max(5.0f, (((float)width) * 0.5f + 0.5f) * aspectMultiplier, (((float)height) * 0.5f + 0.5f) * aspectMultiplier);

            // count number of correct tiles initially
            CountCorrectTiles();
            defaultCorrectTiles = correctTiles;
        } catch {
            Debug.Log("Invalid level information.");
        }
    }
	
	/// <summary>
    /// Checks for input and if the puzzle is complete.
    /// </summary>
	void Update () {
        if (!puzzleFinished) {
            if (Input.GetButtonDown("Pause")) {  // open pause menu
                Pause();
            }

            // show/hide solution
            if (!pauseMenu.activeSelf && Input.GetButtonDown("ShowSolution")) {
                solutionGrid.SetActive(true);
            } else if (Input.GetButtonUp("ShowSolution")) {
                solutionGrid.SetActive(false);
            }
        }

        if (Input.GetButtonDown("Reset")) {     // reset puzzle
            Reset();
        } 
	}

    /// <summary>
    /// Pauses or unpauses the game.
    /// </summary>
    public void Pause() {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        solutionGrid.SetActive(false);
        Player p = player.GetComponent<Player>();
        p.CanMove = !p.CanMove;
        p.Paused = !p.Paused;

        // pause/unpause each tool animation
        foreach (KeyValuePair<IntVector.IntVector2, Tool> t in gridTools) {
            t.Value.Pause();
        }
    }

    /// <summary>
    /// Creates the colour map.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<ColourPicker.Colour, Color> CreateColourMap() {
        Dictionary<ColourPicker.Colour, Color> cm = new Dictionary<ColourPicker.Colour, Color>();
        cm.Add(ColourPicker.Colour.Red, new Color(1.0f, 33.0f / 255.0f, 33.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Blue, new Color(0.0f, 114.0f / 255.0f, 188.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Yellow, new Color(253.0f / 255.0f, 253.0f / 255.0f, 34.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Orange, new Color(1.0f, 111.0f / 255.0f, 24.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Purple, new Color(170.0f / 255.0f, 79.0f / 255.0f, 238.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Green, new Color(52.0f / 255.0f, 204.0f / 255.0f, 63.0f / 255.0f));
        cm.Add(ColourPicker.Colour.White, Color.white);
        return cm;
    }

    /// <summary>
    /// Creates the ice colour map.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<ColourPicker.Colour, Color> CreateIceColourMap() {
        Dictionary<ColourPicker.Colour, Color> cm = new Dictionary<ColourPicker.Colour, Color>();
        cm.Add(ColourPicker.Colour.Red, new Color(1.0f, 113.0f / 255.0f, 113.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Blue, new Color(108.0f / 255.0f, 163.0f / 255.0f, 199.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Yellow, new Color(1.0f, 1.0f, 148.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Orange, new Color(1.0f, 161.0f / 255.0f, 105.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Purple, new Color(193.0f / 255.0f, 135.0f / 255.0f, 236.0f / 255.0f));
        cm.Add(ColourPicker.Colour.Green, new Color(124.0f / 255.0f, 221.0f / 255.0f, 131.0f / 255.0f));
        cm.Add(ColourPicker.Colour.White, Color.white);
        return cm;
    }

    /// <summary>
    /// Resets the puzzle grid to its default state. Moves player to default location and removes colour.
    /// </summary>
    public void Reset() {
        // reset grid tile colours
        for (int i = 0; i < width; i += 1) {
            for (int j = 0; j < height; j += 1) {
                gridTiles[i][j].Colour = ColourPicker.Colour.White;
            }
        }

        // reset number of correct tiles
        correctTiles = defaultCorrectTiles;

        // reset player
        player.GetComponent<Player>().Reset(playerStartLocation);
        playerCurrentLocation = playerStartLocation;
        cursor.transform.localPosition = new Vector2((float)playerStartLocation.x, (float)playerStartLocation.y);

        // hide win message
        winMessage.Hide();

        // other initial settings
        puzzleFinished = false;
        moveCount.Reset();
        pauseMenu.SetActive(false);
        Player p = player.GetComponent<Player>();
        p.CanMove = true;
        p.Paused = false;


        // turn on tool animation
        foreach (KeyValuePair<IntVector.IntVector2, Tool> t in gridTools) {
            t.Value.TurnOnAnimation();
        }
    }

    /// <summary>
    /// Counts the number of correct tiles on the puzzle grid.
    /// </summary>
    private void CountCorrectTiles() {
        correctTiles = 0;
        for (int i = 0; i < width; i += 1) {
            for (int j = 0; j < height; j += 1) {
                if (gridTiles[i][j].Colour == solutionTiles[i][j].Colour) {
                    correctTiles += 1;
                }
            }
        }
    }

    /// <summary>
    /// Moves the player across the grid one unit in the given direction. Colours the tile at that location.
    /// </summary>
    /// <param name="d">Direction to move the player in.</param>
    /// <param name="col">Colour of the player.</param>
    public void MovePlayer(Direction d) {
        Vector2 cursorPos = cursor.transform.localPosition;
        if (d == Direction.Up) {
            playerCurrentLocation.y += 1;
            cursorPos.y += 1;
        } else if (d == Direction.Down) {
            playerCurrentLocation.y -= 1;
            cursorPos.y -= 1;
        } else if (d == Direction.Left) {
            playerCurrentLocation.x -= 1;
            cursorPos.x -= 1;
        } else {
            playerCurrentLocation.x += 1;
            cursorPos.x += 1;
        }

        cursor.transform.localPosition = cursorPos;

        // check if the player has activated a tool
        if (gridTools.ContainsKey(playerCurrentLocation)) {
            gridTools[playerCurrentLocation].PerformAction(player.GetComponent<Player>());
        }

        // colour or erase the tile
        UpdatePlayerTile();
    }

    /// <summary>
    /// Colours or erases the tile that the player is currently on.
    /// </summary>
    public void UpdatePlayerTile() {
        Player p = player.GetComponent<Player>();
        if (p.EraserOn) {
            EraseColour(playerCurrentLocation);
        } else {
            ColourTile(playerCurrentLocation, p.Colour);
        }
    }

    /// <summary>
    /// Colours the tile at the given position with the given colour.
    /// </summary>
    /// <param name="pos">Position of the tile.</param>
    /// <param name="col">Colour to use.</param>
    public void ColourTile(IntVector.IntVector2 pos, ColourPicker.Colour col) {
        // only default and ice tiles can be painted, colour of white will not affect tiles
        if ((gridTiles[pos.x][pos.y].Type == Tile.TileType.Default || gridTiles[pos.x][pos.y].Type == Tile.TileType.Ice) && col != ColourPicker.Colour.White) {

            // check if colour change affects currently correct tiles
            UpdateCorrectTiles(pos, col);

            gridTiles[pos.x][pos.y].Colour = col;
        }
    }

    /// <summary>
    /// Gets the tile type of the tile at the given location.
    /// </summary>
    /// <param name="loc">Location of the tile.</param>
    /// <returns></returns>
    public Tile.TileType GetTileTypeAt(IntVector.IntVector2 loc) {
        return gridTiles[loc.x][loc.y].Type;
    }

    /// <summary>
    /// Colours the tiles around the given location.
    /// </summary>
    /// <param name="col">Colour to use.</param>
    /// <param name="location">Location of the center of the splash.</param>
    /// <param name="radius">Radius of the splash. Must be greater than zero.</param>
    public void Splash(IntVector.IntVector2 location, int radius, ColourPicker.Colour col) {
        IntVector.IntVector2 pos = location;
        for (int i = -radius; i <= radius; i += 1) {
            pos.x = location.x + i;
            for (int j = -radius; j <= radius; j += 1) {
                pos.y = location.y + j;
                if (withinGrid(pos) && (Mathf.Abs(i) + Mathf.Abs(j) <= radius)) {
                    ColourTile(pos, col);
                }
            }
        }
    }

    /// <summary>
    /// Colours a line of tiles starting from the given location and moving in the given direction. Line stops at blank tiles.
    /// </summary>
    /// <param name="loc">Location to start colouring from.</param>
    /// <param name="dir">Direction to move in.</param>
    /// <param name="col">Colour to use.</param>
    public void ColourLineFrom(IntVector.IntVector2 loc, Direction dir, ColourPicker.Colour col) {
        int dx = 0;
        int dy = 0;

        if (dir == Direction.Up) {
            dy = 1;
        } else if (dir == Direction.Down) {
            dy = -1;
        } else if (dir == Direction.Left) {
            dx = -1;
        } else if (dir == Direction.Right) {
            dx = 1;
        }

        while (loc.x >= 0 && loc.x < width && loc.y >= 0 && loc.y < height) {
            if (gridTiles[loc.x][loc.y].Type == Tile.TileType.Blank) { // stop at blanks
                break;
            }
            ColourTile(loc, col);
            loc.x += dx;
            loc.y += dy;
        }
    }

    /// <summary>
    /// Removes the colour of a tile at the specified position.
    /// </summary>
    /// <param name="pos">Location of the tile.</param>
    public void EraseColour(IntVector.IntVector2 pos) {
        // check if erasing affects correct tiles
        UpdateCorrectTiles(pos, ColourPicker.Colour.White);

        gridTiles[pos.x][pos.y].Colour = ColourPicker.Colour.White;
    }

    /// <summary>
    /// Updates the number of correct tiles when changing the colour of one tile.
    /// </summary>
    /// <param name="pos">Location of tile.</param>
    /// <param name="col">Colour that the tile will change to.</param>
    private void UpdateCorrectTiles(IntVector.IntVector2 pos, ColourPicker.Colour col) {
        if (gridTiles[pos.x][pos.y].Colour == solutionTiles[pos.x][pos.y].Colour && col != solutionTiles[pos.x][pos.y].Colour) { // changing colour of tile that is already correct
            correctTiles -= 1;
        } else if (gridTiles[pos.x][pos.y].Colour != solutionTiles[pos.x][pos.y].Colour && col == solutionTiles[pos.x][pos.y].Colour) { // changing tile to correct colour
            correctTiles += 1;
        }
    }

    /// <summary>
    /// Teleports the player to the given position on the grid.
    /// </summary>
    /// <param name="loc">Location on the grid.</param>
    public void TeleportPlayer(IntVector.IntVector2 loc) {
        playerCurrentLocation = loc;
        cursor.transform.localPosition = new Vector2((float)playerCurrentLocation.x, (float)playerCurrentLocation.y);
        player.GetComponent<Player>().Teleport(loc);
    }

    /// <summary>
    /// Determines the location of the player after moving in a certain direction.
    /// </summary>
    /// <param name="d">Direction the player wants to move in.</param>
    /// <returns>A vector containing the expected new position of the player (x, y components),
    /// and the number of units the player has to move to get there (z component). </returns>
    public IntVector.IntVector3 NewPlayerLocation(Direction d) {
        // default return vector is the current location with distance of 0
        IntVector.IntVector3 newLoc = new IntVector.IntVector3(playerCurrentLocation.x, playerCurrentLocation.y, 0);

        // check if the player can move and update the vector
        if (d == Direction.Up) {
            newLoc = NewPlayerLocationHelper(newLoc, 0, 1);
        } else if (d == Direction.Down) {
            newLoc = NewPlayerLocationHelper(newLoc, 0, -1);
        } else if (d == Direction.Left) {
            newLoc = NewPlayerLocationHelper(newLoc, -1, 0);
        } else if (d == Direction.Right) {
            newLoc = NewPlayerLocationHelper(newLoc, 1, 0);
        }

        return newLoc;
    }

    /// <summary>
    /// Checks if the given location is in the bounds of the puzzle grid.
    /// </summary>
    /// <param name="location">The position of the potential tile.</param>
    /// <returns></returns>
    private bool withinGrid(IntVector.IntVector2 location) {
        return (location.x >= 0 && location.x < width && location.y >= 0 && location.y < height);
    }

    /// <summary>
    /// Check if the puzzle is finished. If it is, perform appropriate action.
    /// </summary>
    public void CheckForFinishedPuzzle() {
        if ((correctTiles == (width * height)) && !puzzleFinished) {
            winMessage.ShowMessage();
            puzzleFinished = true;
            player.GetComponent<Player>().CanMove = false;
            solutionGrid.SetActive(false);
        }
    }

    /// <summary>
    /// Helper function for NewPlayerLocation function. Determines the player's new location when moving.
    /// </summary>
    /// <param name="loc">Vector containing the location of the player (x, y component) and the distance it will travel (z component).</param>
    /// <param name="dx">Change in x value when travelling.</param>
    /// <param name="dy">Change in y value when travelling.</param>
    /// <returns></returns>
    private IntVector.IntVector3 NewPlayerLocationHelper(IntVector.IntVector3 loc, int dx, int dy) {
        while (loc.x + dx >= 0 && loc.x + dx < width && loc.y + dy >= 0 && loc.y + dy < height) { // check if new location is on the grid
            if (gridTiles[loc.x + dx][loc.y + dy].Type == Tile.TileType.Default ||      // stop on default and dark tiles
                gridTiles[loc.x + dx][loc.y + dy].Type == Tile.TileType.Dark) {
                loc.x += dx;
                loc.y += dy;
                loc.z += 1;
                break;
            } else if (gridTiles[loc.x + dx][loc.y + dy].Type == Tile.TileType.Ice) {   // slide on ice tiles
                loc.x += dx;
                loc.y += dy;
                loc.z += 1;

                IntVector.IntVector2 tmp = new IntVector.IntVector2(loc.x, loc.y);
                if (gridTools.ContainsKey(tmp) && gridTools[tmp].CanRedirect()) {
                    break;
                }
            } else {   // stop before going onto blank tiles
                break;
            }
        }

        return loc;
    }

    /// <summary>
    /// Reads in a char and returns the appropriate tile type.
    /// </summary>
    /// <param name="t">Char representing the tile.</param>
    /// <returns>Return the tile type. If tile is not able to be determined, just return blank tile.</returns>
    private Tile.TileType ReadTileType(char t) {
        switch(t) {
            case 'd': return Tile.TileType.Default;
            case 'i': return Tile.TileType.Ice;
            case 'r': return Tile.TileType.Dark;
        }

        return Tile.TileType.Blank;
    }

    /// <summary>
    /// Reads in a char and returns the appropriate colour.
    /// </summary>
    /// <param name="c">Char representing the colour.</param>
    /// <returns></returns>
    private ColourPicker.Colour ReadColour(char c) {
        switch(c) {
            case 'r': return ColourPicker.Colour.Red;
            case 'b': return ColourPicker.Colour.Blue;
            case 'y': return ColourPicker.Colour.Yellow;
            case 'o': return ColourPicker.Colour.Orange;
            case 'p': return ColourPicker.Colour.Purple;
            case 'g': return ColourPicker.Colour.Green;
        }

        return ColourPicker.Colour.White;
    }

    /// <summary>
    /// Reads in a char and returns the appropriate direction.
    /// </summary>
    /// <param name="c">Char representing the direction.</param>
    /// <returns></returns>
    private Direction ReadDirection(char c) {
        switch(c) {
            case 'u': return Direction.Up;
            case 'd': return Direction.Down;
            case 'l': return Direction.Left;
        }

        return Direction.Right;
    }
}
