using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public GameObject unit;
    public HexGrid hexGrid;

    private Vector3 genPoint;
    private static float xOffset = 2, yOffset = 1, zOffset = 1.73f;

    private void Awake()
    {
        if (name == "CastleBlue")
            genPoint = transform.position + new Vector3(1f, 0f, 1.73f);
        else
            genPoint = transform.position + new Vector3(-1f, 0f, -1.73f);

        GetFloor();
    }

    private void Start()
    {
        StartCoroutine(GenUnit());
    }

    private void Update()
    {
        
    }

    private void GetFloor()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Hextile")))
        {
            hit.collider.GetComponent<Hex>().ThisIsOntheFloor(gameObject);
        }
    }

    private IEnumerator GenUnit()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            Instantiate(unit, genPoint, Quaternion.identity);
            yield return new WaitForSeconds(2.5f);
        }
    }
}
