using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the game object pulse in size.
/// </summary>
public class SizePulser : MonoBehaviour {

    /// <summary>
    /// True if the object is growing, false otherwise.
    /// </summary>
    private bool grow = true;

    /// <summary>
    /// Amount to lerp.
    /// </summary>
    private float lerpAmount = 0.5f;

    /// <summary>
    /// Amount to change growth every frame.
    /// </summary>
    private float growthAmount = 0.6f;

    /// <summary>
    /// Scale of smallest size.
    /// </summary>
    private Vector2 smallScale = new Vector2(1.0f, 1.0f);

    /// <summary>
    /// Scale of biggest size.
    /// </summary>
    private Vector2 bigScale = new Vector2(1.0f, 1.0f);

    /// <summary>
    /// Is the game paused?
    /// </summary>
    private bool paused = false;

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

    // Does pulsing
    void Update () {
        if (!paused) {
            transform.localScale = Vector2.Lerp(smallScale, bigScale, lerpAmount);

            // Update lerp amount
            if (lerpAmount >= 1.0f && grow) {
                grow = false;
            } else if (lerpAmount <= 0.0f && !grow) {
                grow = true;
            } else if (grow) {
                lerpAmount += growthAmount * Time.deltaTime;
            } else {
                lerpAmount -= growthAmount * Time.deltaTime;
            }
        }
	}

    /// <summary>
    /// Sets size pulser properties.
    /// </summary>
    /// <param name="g">Is the object growing initially?</param>
    /// <param name="la">Lerp amount. Between 0 and 1.</param>
    /// <param name="ga">Growth amount.</param>
    public void SetParams(bool g, float la, Vector2 small, Vector2 big, float ga = 0.6f) {
        grow = g;
        lerpAmount = la;
        growthAmount = ga;
        smallScale = small;
        bigScale = big;
    }
}
