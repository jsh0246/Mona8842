using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexDirection
{
    public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>
    {
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 0, 0)
    };

    public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0)
    };
}