using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hex좌표의 가장 기본인 HexCoordinate를 멤버변수로 가지고 있다.
// HexCoordinate를 capsule화해서 씀
[SelectionBase]
public class Hex : MonoBehaviour
{
    [SerializeField]
    private GlowHighlight highlight;
    private HexCoordinates hexCoordinate;

    [SerializeField]
    private GameObject isOntheFloor;

    public Vector3Int HexCoords => hexCoordinate.GetHexCoords();

    private void Awake()
    {
        highlight = GetComponent<GlowHighlight>();
        hexCoordinate = GetComponent<HexCoordinates>();
        isOntheFloor = null;
    }

    public void EnableHighlight()
    {
        highlight.ToggleGlow(true);
    }

    public void DisableHighlight()
    {
        highlight.ToggleGlow(false);
    }

    public void ThisIsOntheFloor(GameObject o)
    {
        isOntheFloor = o;
    }

    public GameObject WhatIsOntheFloor()
    {
        return isOntheFloor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("UnitBlue") || other.CompareTag("UnitRed"))
        {
            isOntheFloor = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UnitBlue") || other.CompareTag("UnitRed"))
        {
            isOntheFloor = null;
        }
    }
}
