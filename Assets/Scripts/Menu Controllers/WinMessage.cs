using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinMessage : MonoBehaviour {
    /// <summary>
    /// Level select button.
    /// </summary>
    public GameObject levelSelectButton;

    /// <summary>
    /// Next level button.
    /// </summary>
    public GameObject nextLevelButton;

    /// <summary>
    /// Black background.
    /// </summary>
    public GameObject bg;

    /// <summary>
    /// Win message.
    /// </summary>
    public GameObject msg;

    /// <summary>
    /// Sets up the win screen.
    /// </summary>
    public void ShowMessage() {
        if (Level.IsAtMax()) {  // no next level button if we are at last level
            RectTransform lsRect = levelSelectButton.GetComponent<RectTransform>();
            Vector2 anchor = lsRect.anchorMin;
            anchor.x = 0.4f;
            lsRect.anchorMin = anchor;
            anchor = lsRect.anchorMax;
            anchor.x = 0.6f;
            lsRect.anchorMax = anchor;
        }

        // Hide all elements initially
        levelSelectButton.SetActive(false);
        nextLevelButton.SetActive(false);
        bg.SetActive(false);
        msg.SetActive(false);
        gameObject.SetActive(true);

        // start transition
        StartCoroutine("MessageTransition");
    }

    /// <summary>
    /// Transitions to the win message.
    /// </summary>
    IEnumerator MessageTransition() {
        // transition bg
        #region BG Transition
        RectTransform rect = bg.GetComponent<RectTransform>();
        Vector2 v = rect.anchorMin;
        v.y = 1.1f;
        rect.anchorMin = v;
        float transitionRate = 0.1f;
        bg.SetActive(true);

        while (v.y > -0.1f) {
            v.y -= transitionRate * Time.deltaTime;
            rect.anchorMin = v;
            transitionRate += 0.25f;
            yield return null;
        }

        // wait a little
        yield return new WaitForSeconds(.1f);
        #endregion

        // transition message
        #region Message Transition
        rect = msg.GetComponent<RectTransform>();
        v = new Vector2(0.0f, 0.0f);
        rect.localScale = v;
        transitionRate = 0.2f;
        msg.SetActive(true);

        while (v.x < 1.0f) {
            v.x = v.y = v.x + (transitionRate * Time.deltaTime);
            rect.localScale = v;

            if (v.x > 0.5f && transitionRate > 0.2f) {
                transitionRate -= 0.15f;
            } else {
                transitionRate += 0.35f;
            }
            yield return null;
        }

        // wait a little
        yield return new WaitForSeconds(.1f);
        #endregion

        // transition buttons
        #region Button Transition
        float alpha = 0.0f;
        CanvasRenderer levelCR = levelSelectButton.GetComponent<CanvasRenderer>();
        CanvasRenderer nextCR = nextLevelButton.GetComponent<CanvasRenderer>();

        if (Level.IsAtMax()) {  // don't fade in next level button
            levelCR.SetAlpha(alpha);
            levelSelectButton.SetActive(true);

            while (alpha < 1.0f) {
                alpha += 1.5f * Time.deltaTime;
                levelCR.SetAlpha(alpha);
                yield return null;
            }
        } else {
            levelCR.SetAlpha(alpha);
            nextCR.SetAlpha(alpha);
            levelSelectButton.SetActive(true);
            nextLevelButton.SetActive(true);

            while (alpha < 1.0f) {
                alpha += 1.5f * Time.deltaTime;
                levelCR.SetAlpha(alpha);
                nextCR.SetAlpha(alpha);
                yield return null;
            }
        }
        #endregion

    }

    /// <summary>
    /// Hides the win message.
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }
}
