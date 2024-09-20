using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorUnit : MonoBehaviour
{
    private Tilemap tileMap;
    private TileBase genPoint;

    private void Awake()
    {
        tileMap = GameManager.Instance.tileMap;
    }

    private void Start()
    {
        SiteGenPoint();
    }


    private void SiteGenPoint()
    {

        Vector3 genPos = transform.position + Vector3.right * 3 + Vector3.forward * 3;
        Vector3Int genPointTile = tileMap.WorldToCell(genPos);
        print(genPos);
        print(genPointTile);
        Vector3Int vint = new Vector3Int(0, 0, 0);
        print(tileMap.GetInstantiatedObject(vint));
        print(tileMap.GetTile(vint).ToString());
        print(tileMap.GetTile(vint).GameObject());
        //print(genPointTile);
        //print(tileMap.CellToWorld(genPointTile));

    }
}
