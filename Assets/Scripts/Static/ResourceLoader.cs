using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader {

    /// <summary>
    /// Creates a new game object with a sprite.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <param name="parent">Parent of the object.</param>
    /// <param name="localx">Local x coordinate.</param>
    /// <param name="localy">Local y coordinate.</param>
    /// <param name="sortLayer">Sorting layer of the sprite.</param>
    /// <param name="sortOrder">Sorting order of the sprite in the sorting layer.</param>
    /// <param name="path">Pathname for the sprite image.</param>
    /// <param name="col">Colour of the sprite.</param>
    /// <returns></returns>
    public static GameObject GetSpriteGameObject(string name, GameObject parent, float localx, float localy, string sortLayer, int sortOrder, string path, ColourPicker.Colour col = ColourPicker.Colour.White) {
        GameObject g = new GameObject();
        g.name = name;
        g.transform.parent = parent.transform;
        g.transform.localPosition = new Vector2(localx, localy);
        g.AddComponent<SpriteRenderer>();
        SpriteRenderer render = g.GetComponent<SpriteRenderer>();
        render.sortingLayerName = sortLayer;
        render.sortingOrder = sortOrder;
        render.color = Grid.ColourMap[col];
        render.sprite = ResourceLoader.GetSprite(path);
        return g;
    }

    /// <summary>
    /// Returns the sprite at the given path.
    /// </summary>
    /// <param name="path">Pathname where the sprite is located.</param>
    /// <returns>Sprite of tile at given location.</returns>
    public static Sprite GetSprite(string path) {
        Texture2D tex = Resources.Load(path) as Texture2D;
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
    }
}
