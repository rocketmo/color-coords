using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player behaviour.
/// </summary>
public class Player : MonoBehaviour {

    /// <summary>
    /// Grid that the player is on.
    /// </summary>
    public Grid grid;

    /// <summary>
    /// Outline of the player.
    /// </summary>
    public SpriteRenderer outline;

    /// <summary>
    /// Is the player moving?
    /// </summary>
    private bool isMoving;

    /// <summary>
    /// Will the player move again when their initial move is over?
    /// </summary>
    private bool extraMove;

    /// <summary>
    /// The direction of the extra move.
    /// </summary>
    private Grid.Direction extraMoveDirection;

    /// <summary>
    /// Player's colour.
    /// </summary>
    private ColourPicker.Colour colour;

    /// <summary>
    /// Player's colour.
    /// </summary>
    public ColourPicker.Colour Colour {
        get {
            return colour;
        }

        set {
            colour = value;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Grid.ColourMap[colour];

            if (eraserOn) {  // turn off eraser
                eraserOn = false;
                outline.sprite = ResourceLoader.GetSprite("Sprites/Player/Player-Outline");
            }
        }
    }

    /// <summary>
    /// Can the player move?
    /// </summary>
    private bool canMove;

    /// <summary>
    /// Can the player move?
    /// </summary>
    public bool CanMove {
        get {
            return canMove;
        }

        set {
            canMove = value;
        }
    }

    /// <summary>
    /// True if the player will erase tiles instead of colour.
    /// </summary>
    private bool eraserOn;

    /// <summary>
    /// True if the player will erase tiles instead of colour.
    /// </summary>
    public bool EraserOn {
        get {
            return eraserOn;
        }
    }

    /// <summary>
    /// Number of moves the player has taken.
    /// </summary>
    public MoveCounter moveCount;

    /// <summary>
    /// True if the player is to be reset to its original state.
    /// </summary>
    private bool reset;

    /// <summary>
    /// Last direction the player moved in.
    /// </summary>
    private Grid.Direction lastMove;

    /// <summary>
    /// Last direction the player moved in.
    /// </summary>
    public Grid.Direction LastMove {
        get {
            return lastMove;
        }
    }

    /// <summary>
    /// Is the game paused?
    /// </summary>
    private bool paused;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    public bool Paused {
        get {
            return paused;
        }

        set {
            paused = value;
        }
    }

    /// <summary>
    /// Initialization
    /// </summary>
    void Start() {
        isMoving = false;
        colour = ColourPicker.Colour.White;
        canMove = true;
        eraserOn = false;
        lastMove = extraMoveDirection = Grid.Direction.Left;
        extraMove = false;
        reset = false;
        paused = false;
    }

    /// <summary>
    /// Handles player input and movement every frame.
    /// </summary>
    void Update() {
        // check for player movement
        if (!isMoving && canMove) { // input is ignored if player is currently moving
            if (extraMove) {
                extraMove = false;
                CheckMovement(extraMoveDirection);
            } else {
                if (Input.GetAxisRaw("Vertical") > 0.0f) {
                    CheckMovement(Grid.Direction.Up);
                    lastMove = Grid.Direction.Up;
                } else if (Input.GetAxisRaw("Vertical") < 0.0f) {
                    CheckMovement(Grid.Direction.Down);
                    lastMove = Grid.Direction.Down;
                } else if (Input.GetAxisRaw("Horizontal") < 0.0f) {
                    CheckMovement(Grid.Direction.Left);
                    lastMove = Grid.Direction.Left;
                } else if (Input.GetAxisRaw("Horizontal") > 0.0f) {
                    CheckMovement(Grid.Direction.Right);
                    lastMove = Grid.Direction.Right;
                }
            }
        }
    }

    /// <summary>
    /// Enables the player to move again after the inital move is over.
    /// </summary>
    /// <param name="d">Direction the next move will be.</param>
    public void MakeExtraMove(Grid.Direction d) {
        extraMove = true;
        extraMoveDirection = d;
    }

    /// <summary>
    /// The player will now eraser instead of colour.
    /// </summary>
    public void BeginErase() {
        eraserOn = true;
        colour = ColourPicker.Colour.White;
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        s.color = Color.white;
        outline.sprite = ResourceLoader.GetSprite("Sprites/Player/Player-Eraser-Outline");
    }

    /// <summary>
    /// Checks if the player can move in a certain direction. If they can, move them there.
    /// </summary>
    /// <param name="d">Direction the player will move in.</param>
    private void CheckMovement(Grid.Direction d) {
        IntVector.IntVector3 newLoc = grid.NewPlayerLocation(d);
        if (newLoc.z > 0) {
            isMoving = true;
            lastMove = d;
            StartCoroutine(Move(newLoc, d));
        }
    }

    /// <summary>
    /// Smoothly moves the player to the given position.
    /// </summary>
    /// <param name="newLoc">Contains the new location for the player (x, y component) and the distance the player must travel (z component).</param>
    /// <param name="d">Direction the player is moving in.</param>
    private IEnumerator Move(IntVector.IntVector3 newLoc, Grid.Direction d) {
        Vector2 startPos = transform.localPosition;                         // start location
        Vector2 endPos = new Vector2((float) newLoc.x, (float) newLoc.y);   // end location
        float moveTimer = 0.0f;                                             // time elapsed during move
        float moveTimeLimit = 0.12f * ((float) newLoc.z + 1);               // total time to finish movement
        float ratio = 0.0f;                                                 // lerp ratio
        float unitCounter = 0.0f;                                           // checks if player has moved a unit

        // move player
        while (ratio < 1.0f) {
            if (!paused) {
                if (reset) {    // check if reset button was pressed
                    reset = false;
                    break;
                }

                float prev = ratio;
                moveTimer += Time.deltaTime;
                ratio = moveTimer / moveTimeLimit;
                transform.localPosition = Vector2.Lerp(startPos, endPos, ratio);

                // check if we've moved a unit
                unitCounter += (ratio - prev) * ((float)newLoc.z);
                if (unitCounter >= 1.0f) {
                    grid.MovePlayer(d);
                    unitCounter -= 1.0f;
                }
            }

            yield return null;
        }

        // player no longer moving
        isMoving = false;

        // check if the puzzle is completed
        if (!extraMove) {
            moveCount.Increment();
        }
        grid.CheckForFinishedPuzzle();
    }

    /// <summary>
    /// Teleports the player to a new location.
    /// </summary>
    /// <param name="loc">New location of the grid.</param>
    public void Teleport(IntVector.IntVector2 loc) {
        Vector3 location = transform.localPosition;
        location.x = loc.x;
        location.y = loc.y;
        transform.localPosition = location;
    }

    /// <summary>
    /// Resets the player to inital conditions.
    /// </summary>
    /// <param name="startLoc">The starting location of the player.</param>
    public void Reset(IntVector.IntVector2 startLoc) {
        Colour = ColourPicker.Colour.White;
        canMove = true;
        transform.localPosition = new Vector2((float)startLoc.x, (float)startLoc.y);

        if (isMoving) {
            reset = true;
        }
    }
}
