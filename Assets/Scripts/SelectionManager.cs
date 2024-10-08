using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    public LayerMask selectionMask;
    public HexGrid hexGrid;

    private List<Vector3Int> neighbors = new List<Vector3Int>();

    private void Awake()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void HandleClick(Vector3 mousePosition)
    {
        GameObject result;
        if (FindTarget(mousePosition, out result))
        {
            Hex selectedHex = result.GetComponent<Hex>();

            selectedHex.DisableHighlight();
            foreach (Vector3Int neighbor in neighbors)
            {
                hexGrid.GetTileAt(neighbor).DisableHighlight();
            }

            neighbors = hexGrid.GetNeighboursFor(selectedHex.HexCoords);

            foreach (Vector3Int neighbor in neighbors)
            {
                hexGrid.GetTileAt(neighbor).EnableHighlight();
            }

            //print($"Neighbours for {selectedHex.HexCoords} are : ");
            //foreach (Vector3Int neighbourPos in neighbors)
            //{
            //    print(neighbourPos);
            //}
        }
    }

    private bool FindTarget(Vector3 mousePosition, out GameObject result)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectionMask))
        {
            result = hit.collider.gameObject;
            return true;
        }

        result = null;
        return false;
    }
}