using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void EnableHighlight()
    {
        highlight.ToggleGlow(true);
    }

    public void DisableHighlight()
    {
        highlight.ToggleGlow(false);
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
