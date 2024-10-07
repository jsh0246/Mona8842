using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hex좌표의 가장 기본 클래스, ConverPositionToOffset에서 현재 좌표를 받아서 Hex좌표로 변경한다.
public class HexCoordinates : MonoBehaviour
{
    public static float xOffset = 2, yOffset = 1, zOffset = 1.73f;

    [Header("Offset Coordinates")]
    [SerializeField]
    private Vector3Int offsetCoordinates;

    internal Vector3Int GetHexCoords() => offsetCoordinates;

    private void Awake()
    {
        offsetCoordinates = ConverPositionToOffset(transform.position);
    }

    private Vector3Int ConverPositionToOffset(Vector3 position)
    {
        int x = Mathf.CeilToInt(position.x / xOffset);
        int y = Mathf.RoundToInt(position.y / yOffset);
        int z = Mathf.RoundToInt(position.z / zOffset);

        return new Vector3Int(x, y, z);
    }
}
