using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hex��ǥ�� Hex�� ��ųʸ��� ��´�
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
    {   // hexCoordinate�� �̿��� ����
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

    public List<Vector3Int> GetNeighboursForExceptObstacles(Vector3Int hexCoordinates)
    {   // hexCoordinate�� �̿��� ����
        if (hexTileDict.ContainsKey(hexCoordinates) == false)
            return new List<Vector3Int>();

        if (hexTileNeighboursDict.ContainsKey(hexCoordinates))
            return hexTileNeighboursDict[hexCoordinates];

        hexTileNeighboursDict.Add(hexCoordinates, new List<Vector3Int>());

        foreach (Vector3Int direction in Direction.GetDirectionVector(hexCoordinates.z))
        {
            if (hexTileDict.ContainsKey(hexCoordinates + direction))
            {
                if(hexTileDict[hexCoordinates + direction].WhatIsOntheFloor() == null)
                {
                    hexTileNeighboursDict[hexCoordinates].Add(hexCoordinates + direction);
                }
            }
        }

        return hexTileNeighboursDict[hexCoordinates];
    }

    // hexTileNeighboursDict[hexCoordinate] != null ���� ��������, �� GetNeighboursFor(hexCoordinate)�� ����Ǿ� ��ųʸ��� �ϼ��Ǿ� �־����
    //public Dictionary<Vector3Int, GameObject> IsEnemyNeighbouring(Vector3Int hexCoordinates)
    //{
    //    Dictionary<Vector3Int, GameObject> neighbouringObj = new Dictionary<Vector3Int, GameObject>();

    //    foreach(Vector3Int hex in hexTileNeighboursDict[hexCoordinates])
    //    {
    //        GameObject nUnit = GetTileAt(hex).WhatIsOntheFloor();
    //        if (nUnit != null)
    //        {
    //            neighbouringObj[hex] = nUnit;
    //        }
    //    }

    //    return neighbouringObj;
    //}

    // �̿�ã��, �������, �������� ������ ã��
    public void WhoisMyNeighbour(Vector3Int hexCoordinates, ref List<Unit> unitBlue, ref List<Unit> unitRed)
    {
        unitBlue.Clear();
        unitRed.Clear();
        foreach (Vector3Int hex in hexTileNeighboursDict[hexCoordinates])
        {
            GameObject o = GetTileAt(hex).WhatIsOntheFloor();
            
            // tag 2�� �� ���� ���̾�� obstacles�� �ƴϰ� �ѹ��� ���ϰ� ������ �߾ȵǳ� ���� ������
            if (o != null && (o.tag == "UnitBlue" || o.tag == "UnitRed"))
            //if (o != null && o.layer != LayerMask.NameToLayer("Obstacles"))
            {
                //print(o.name);
                //print(o.layer);
                //print(LayerMask.NameToLayer("Obstacles"));

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

        unitBlue.Sort((x, y) => x.troops < y.troops ? 1 : 0);
        unitRed.Sort((x, y) => x.troops < y.troops ? 1 : 0);
    }

    // ���� �ִ� ��ǥ�� ��ȯ�غ��� �ϴ���
    //public List<Vector3Int> IsEnemyNeighbouring2(Vector3Int hexCoordinates, string myTag)
    //{
    //    List<Vector3Int> neighbouringObj = new List<Vector3Int>();

    //    foreach (Vector3Int hex in hexTileNeighboursDict[hexCoordinates])
    //    {
    //        GameObject nUnit = GetTileAt(hex).WhatIsOntheFloor();
    //        if (nUnit != null && !nUnit.CompareTag(myTag))
    //        {
    //            neighbouringObj.Add(hex);
    //        }
    //    }

    //    return neighbouringObj;
    //}
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