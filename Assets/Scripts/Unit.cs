using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    private Tilemap tileMap;

    private Vector3 goalPos;
    private float speed = 2f;
    private bool movable;

    private readonly int[] dx = { 0, 0, 1, -1 };
    private readonly int[] dz = { 1, -1, 0, 0 };

    private void Awake()
    {
        tileMap = GameManager.Instance.tileMap;
        goalPos = transform.position;
        movable = false;
    }

    private void Start()
    {
        //StartCoroutine(WanderGrid());
        StartCoroutine(WanderHex());
    }

    private void FixedUpdate()
    {
        MoveGrid();
        //PrintCode();
    }

    private void PrintCode()
    {
        //print(transform.position);
        print(tileMap.WorldToCell(transform.position));
    }

    private void MoveGrid()
    {
        if (movable)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.fixedDeltaTime * speed);
        }
    }

    private void MoveHex()
    {
        if (movable)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.fixedDeltaTime * speed);
        }
    }

    private IEnumerator WanderGrid()
    {
        while (true)
        {
            int r = Random.Range(0, 4);
            goalPos.x = transform.position.x + dx[r];
            goalPos.z = transform.position.z + dz[r];
            movable = true;
            yield return new WaitForSecondsRealtime(0.6f);
            movable = false;
        }
    }

    private IEnumerator WanderHex()
    {
        while (true)
        {
            int r = Random.Range(0, 6);

            Vector3Int delta;
            Vector3Int curPos = tileMap.WorldToCell(transform.position);

            if(curPos.y % 2 == 0)
            {
                delta = HexDirection.directionsOffsetOdd[r];
            } else
            {
                delta = HexDirection.directionsOffsetEven[r];
            }

            goalPos.x = tileMap.CellToWorld(curPos + delta).x;
            goalPos.z = tileMap.CellToWorld(curPos + delta).z;

            movable = true;
            yield return new WaitForSecondsRealtime(1.5f);
            movable = false;
        }
    }
}
