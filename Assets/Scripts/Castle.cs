using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public GameObject unit;

    private Vector3 genPoint;


    private void Awake()
    {
        genPoint = transform.position + new Vector3(1f, 0f, 1.73f);
    }

    private void Start()
    {
        StartCoroutine(GenUnit());
    }

    private void Update()
    {
        
    }

    private IEnumerator GenUnit()
    {
        yield return new WaitForSecondsRealtime(1f);

        while (true)
        {
            Instantiate(unit, genPoint, Quaternion.identity);
            yield return new WaitForSecondsRealtime(5f);
        }
    }
}
