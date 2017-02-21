using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the game object every frame.
/// </summary>
public class AutoRotator : MonoBehaviour {

    /// <summary>
    /// Amount to rotate.
    /// </summary>
    private float rotateAmount;

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
    /// Randomizes the rotate amount and the initial rotation.
    /// </summary>
    void Start() {
        transform.Rotate(Vector3.forward, Random.Range(0.0f, 360.0f));
        rotateAmount = Random.Range(-35.0f, -25.0f);
        paused = false;
    }

	/// <summary>
    /// Rotate every frame.
    /// </summary>
	void Update () {
        if (!paused) {
            transform.Rotate(Vector3.forward, rotateAmount * Time.deltaTime);
        }
	}
}
