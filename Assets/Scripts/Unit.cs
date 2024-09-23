using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public TextMeshPro troopsUI;
    public int troops = 100;

    private HexGrid hexGrid;
    private Vector3Int curPos;
    private Vector3 goalPos;
    private float speed = 10f;

    public bool movable = false;
    public bool isBattle = false;
    public Coroutine battleCoroutine;

    private void Awake()
    {
        hexGrid = GameObject.FindAnyObjectByType<HexGrid>();
    }

    private void Start()
    {
        StartCoroutine(WanderHex());
    }

    private void Update()
    {
        troopsUI.text = troops.ToString();
    }


    private void FixedUpdate()
    {
        MoveHex();
    }

    private void MoveHex()
    {
        if (!isBattle && movable)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.fixedDeltaTime * speed);
        }
    }

    private IEnumerator WanderHex()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            List<Vector3Int> neighbours = hexGrid.GetNeighboursFor(curPos);
            int rDir = Random.Range(0, neighbours.Count);

            Vector3 goal = hexGrid.GetTileAt(neighbours[rDir]).transform.position;

            goalPos.x = goal.x;
            goalPos.y = transform.position.y;
            goalPos.z = goal.z;

            movable = true;
            yield return new WaitForSeconds(0.3f);
            movable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hextile"))
        {
            curPos = other.GetComponent<HexCoordinates>().GetHexCoords();
        }

        // tag == UnitRed�϶���??
        if(other.CompareTag(tag))
        {
            Unit otherUnit = other.GetComponent<Unit>();
            if(troops >= otherUnit.troops)
            {
                // troops �� otherUnit.troops�� ���� ���� ��� ���� �״°ǰ�? ����� ����� �Ǵ°� ������ ���ÿ� �� �׾�� �ҰŰ��⵵ �ϰ�, ��Ȯ�� ��Ŀ������ �𸣰���
                troops += otherUnit.troops;
                // ��ü �ִϸ��̼� ���� �ϰ� �ı�
                Destroy(other.gameObject);
            }
        }

        // ������ ����
        if(tag.CompareTo("UnitBlue") == 0 && other.CompareTag("UnitRed") || tag.CompareTo("UnitRed") == 0 && other.CompareTag("UnitBlue"))
        {
            // �ִϸ��̼ǰ� �ڷ�ƾ?
            Unit otherUnit = other.GetComponent<Unit>();
            otherUnit.isBattle = isBattle = true;
            battleCoroutine = StartCoroutine(Battle(otherUnit));
        }
    }

    private IEnumerator Battle(Unit otherUnit)
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (troops >= otherUnit.troops)
            {
                troops -= Mathf.RoundToInt(Mathf.Abs(troops - otherUnit.troops) * 0.05f);
                print(troops);
            }
            else
            {
                troops -= Mathf.RoundToInt(Mathf.Abs(troops - otherUnit.troops) * 0.1f);
                print("\t" + troops);
            }

            // Destroy�� ��ɾ���� ���ĸ� �� �����ؾ� �Ѵ� null ������
            if(Dead())
            {
                // �� �׾��µ� �̱� ������ ����(�ڷ�ƾ) ������� ��
                otherUnit.isBattle = isBattle = false;
                otherUnit.StopCoroutine(otherUnit.battleCoroutine);
                StopCoroutine(battleCoroutine);
                break;
            }
        }

        //movable = true;
    }

    private bool Dead()
    {
        if(troops <= 0)
        {
            Destroy(gameObject);
            return true;
        } else
        {
            return false;
        }
    }
}