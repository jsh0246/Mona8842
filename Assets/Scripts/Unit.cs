using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public int troops = 100;

    private HexGrid hexGrid;
    private Vector3Int curPos;
    private Vector3 goalPos;
    private float speed = 10f;
    private bool movable = false;

    private void Awake()
    {
        hexGrid = GameObject.FindAnyObjectByType<HexGrid>();
    }

    private void Start()
    {
        StartCoroutine(WanderHex());
    }


    private void FixedUpdate()
    {
        MoveHex();
    }

    private void MoveHex()
    {
        if (movable)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.fixedDeltaTime * speed);
        }
    }

    private IEnumerator WanderHex()
    {
        yield return new WaitForSecondsRealtime(1f);

        while (true)
        {
            List<Vector3Int> neighbours = hexGrid.GetNeighboursFor(curPos);
            int rDir = Random.Range(0, neighbours.Count);

            Vector3 goal = hexGrid.GetTileAt(neighbours[rDir]).transform.position;

            goalPos.x = goal.x;
            goalPos.y = transform.position.y;
            goalPos.z = goal.z;

            movable = true;
            yield return new WaitForSecondsRealtime(0.3f);
            movable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hextile"))
        {
            curPos = other.GetComponent<HexCoordinates>().GetHexCoords();
        }

        if(other.CompareTag("UnitBlue"))
        {
            Unit otherUnit = other.GetComponent<Unit>();
            if(troops >= otherUnit.troops)
            {
                troops += otherUnit.troops;
                // 합체 애니메이션 까지 하고 파괴
                Destroy(other.gameObject);
            }
        }
    }
}
