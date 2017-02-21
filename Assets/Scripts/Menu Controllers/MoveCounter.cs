using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCounter : MonoBehaviour {

    /// <summary>
    /// Text component.
    /// </summary>
    private Text movesText;

    /// <summary>
    /// Moves counter.
    /// </summary>
    private int count;

	/// <summary>
    /// Initialization.
    /// </summary>
	void Start () {
        count = 0;
        movesText = GetComponent<Text>();
        movesText.text = "Moves: " + count.ToString();
	}
	
    /// <summary>
    /// Increments the counter by one.
    /// </summary>
    public void Increment() {
        count += 1;
        movesText.text = "Moves: " + count.ToString();
    }

    /// <summary>
    /// Reset the count back to zero.
    /// </summary>
    public void Reset() {
        count = 0;
        movesText.text = "Moves: 0";
    }
}
