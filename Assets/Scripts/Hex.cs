using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Hex : MonoBehaviour
{
    private HexCoordinates hexCoordinate;

    public Vector3Int HexCoords => hexCoordinate.GetHexCoords();

    private void Awake()
    {
        hexCoordinate = GetComponent<HexCoordinates>();
    }
}
