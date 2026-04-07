using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public static int terrainLayerMask = 1 << 6; //layer mask to only test for terrain collisions

    public static bool Linecast(Vector2 start, Vector2 end, int layerMask)
    {
        return Physics2D.Linecast(start, end, layerMask);
    }
}
