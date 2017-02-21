using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vector structs using ints.
/// </summary>
public class IntVector {
    /// <summary>
    /// Vector2 but with ints.
    /// </summary>
    public struct IntVector2 {
        public int x, y;

        public IntVector2(int n1, int n2) {
            x = n1;
            y = n2;
        }
    }

    /// <summary>
    /// Vector3 but with ints.
    /// </summary>
    public struct IntVector3 {
        public int x, y, z;

        public IntVector3(int n1, int n2, int n3) {
            x = n1;
            y = n2;
            z = n3;
        }
    }
}
