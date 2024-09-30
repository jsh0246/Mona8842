using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    Dictionary<Vector3Int, Hex> hexTileDict = new Dictionary<Vector3Int, Hex>();
    Dictionary<Vector3Int, List<Vector3Int>> hexTileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();

    private void Start()
    {
        foreach(Hex hex in FindObjectsByType<Hex>(FindObjectsSortMode.None))
        {
            hexTileDict[hex.HexCoords] = hex;
        }
    }

    public Hex GetTileAt(Vector3Int hexCoordinates)
    {
        Hex ret = null;
        hexTileDict.TryGetValue(hexCoordinates, out ret);
        return ret;
    }

    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates)
    {
        if(hexTileDict.ContainsKey(hexCoordinates) == false)
            return new List<Vector3Int>();

        if(hexTileNeighboursDict.ContainsKey(hexCoordinates))
            return hexTileNeighboursDict[hexCoordinates];

        hexTileNeighboursDict.Add(hexCoordinates, new List<Vector3Int>());

        foreach(Vector3Int direction in Direction.GetDirectionVector(hexCoordinates.z))
        {
            if (hexTileDict.ContainsKey(hexCoordinates + direction))
            {
                hexTileNeighboursDict[hexCoordinates].Add(hexCoordinates + direction);
            }
        }

        return hexTileNeighboursDict[hexCoordinates];
    }

    // hexTileNeighboursDict[hexCoordinate] != null 임을 가정했음, 즉 GetNeighboursFor(hexCoordinate)가 실행되어 딕셔너리가 완성되어 있어야함
    public Dictionary<Vector3Int, GameObject> IsEnemyNeighbouring(Vector3Int hexCoordinates)
    {
        Dictionary<Vector3Int, GameObject> neighbouringObj = new Dictionary<Vector3Int, GameObject>();

        foreach(Vector3Int hex in hexTileNeighboursDict[hexCoordinates])
        {
            GameObject nUnit = GetTileAt(hex).WhatIsOntheFloor();
            if (nUnit != null)
            {
                neighbouringObj[hex] = nUnit;
            }
        }

        return neighbouringObj;
    }

    public void WhoisMyNeighbour(Vector3Int hexCoordinates, ref List<Unit> unitBlue, ref List<Unit> unitRed)
    {
        unitBlue.Clear();
        unitRed.Clear();
        foreach (Vector3Int hex in hexTileNeighboursDict[hexCoordinates])
        {
            GameObject o = GetTileAt(hex).WhatIsOntheFloor();
            if (o != null)
            {
                Unit nUnit = o.GetComponent<Unit>();

                if (nUnit.CompareTag("UnitBlue") && nUnit.troops > 0)
                {
                    unitBlue.Add(nUnit);
                }
                else if (nUnit.CompareTag("UnitRed") && nUnit.troops > 0)
                {
                    unitRed.Add(nUnit);
                }
            }
        }
    }

    // 뭐가 있는 좌표만 반환해보자 일단은
    public List<Vector3Int> IsEnemyNeighbouring2(Vector3Int hexCoordinates, string myTag)
    {
        List<Vector3Int> neighbouringObj = new List<Vector3Int>();

        foreach (Vector3Int hex in hexTileNeighboursDict[hexCoordinates])
        {
            GameObject nUnit = GetTileAt(hex).WhatIsOntheFloor();
            if (nUnit != null && !nUnit.CompareTag(myTag))
            {
                neighbouringObj.Add(hex);
            }
        }

        return neighbouringObj;
    }
}

public static class Direction
{
    public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 1),   // N1
        new Vector3Int(0, 0, 1),    // N2
        new Vector3Int(1, 0, 0),    // E
        new Vector3Int(0, 0, -1),   // S2
        new Vector3Int(-1, 0, -1),  // S1
        new Vector3Int(-1, 0, 0)    // W
    };

    public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>
    {
        new Vector3Int(0, 0, 1),    // N1
        new Vector3Int(1, 0, 1),    // N2
        new Vector3Int(1, 0, 0),    // E
        new Vector3Int(1, 0, -1),   // S2
        new Vector3Int(0, 0, -1),   // S1
        new Vector3Int(-1, 0, 0)    // W
    };

    public static List<Vector3Int> GetDirectionVector(int z)
     => z % 2 == 0 ? directionsOffsetEven : directionsOffsetOdd;
}