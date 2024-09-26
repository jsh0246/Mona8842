using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public enum UnitState { Idle, Run, Fight, Dead, Unite }

    [HideInInspector]
    public UnitState state;
    public TextMeshPro troopsUI;
    public int troops = 100;


    private HexGrid hexGrid;
    private Animator anim;
    private Vector3Int curPos;
    private Vector3 goalPos;
    private float speed = 1f;

    [HideInInspector]
    public bool movable = false;
    [HideInInspector]
    public bool isBattle = false;
    public Coroutine battleCoroutine;

    private void Awake()
    {
        hexGrid = GameObject.FindAnyObjectByType<HexGrid>();
        anim = GetComponent<Animator>();
        state = UnitState.Idle;
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
            anim.SetBool("Run", true);
            transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.fixedDeltaTime * speed);
        }
    }

    private IEnumerator WanderHex()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            List<Vector3Int> neighbours = hexGrid.GetNeighboursFor(curPos);
            Vector3 goal; 
            List<Vector3Int> nUnit = hexGrid.IsEnemyNeighbouring2(curPos, tag);
            //foreach (Vector3Int unit in nUnit)
            //    print(hexGrid.GetTileAt(unit).WhatIsOntheFloor().name);
            //print(unit);

            if (nUnit != null)
            {

                int rDir = Random.Range(0, nUnit.Count);
                Hex enemy = hexGrid.GetTileAt(nUnit[rDir]);

                // FIght
                //StartCoroutine(Battle(enemy.WhatIsOntheFloor().GetComponent<Unit>()));
                //print(enemy.WhatIsOntheFloor().name);
                anim.SetBool("Attack01", true);

                transform.LookAt(enemy.transform.position);
                yield return null;
                GetDamage(enemy.WhatIsOntheFloor().GetComponent<Unit>().troops);

                yield return new WaitForSeconds(2f);
                anim.SetBool("Attack01", false);
            }
            else
            {
                int rDir = Random.Range(0, neighbours.Count);
                goal = hexGrid.GetTileAt(neighbours[rDir]).transform.position;

                goalPos.x = goal.x;
                goalPos.y = transform.position.y;
                goalPos.z = goal.z;

                movable = true;

                yield return new WaitForSeconds(2f);

                movable = false;
                if (!isBattle && !movable)
                {
                    anim.SetBool("Run", false);
                }

                yield return new WaitForSeconds(2f);
            }
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

        /* !! �浹�� ������ �Ǻ����� �ʰ� HexŽ���� ���� ���� �Ǻ����� !!*/
        // Battle�� �̿��˻����� ��������, �浹�� �ƴѵ�
        // ������ ����
        //if(tag.CompareTo("UnitBlue") == 0 && other.CompareTag("UnitRed") || tag.CompareTo("UnitRed") == 0 && other.CompareTag("UnitBlue"))
        //{
        //    // �ִϸ��̼ǰ� �ڷ�ƾ?
        //    Unit otherUnit = other.GetComponent<Unit>();
        //    otherUnit.isBattle = isBattle = true;
        //    battleCoroutine = StartCoroutine(Battle(otherUnit));
        //}
    }

    private void GetDamage(int enemyTroops)
    {
        int troopsDiff = Mathf.Abs(troops - enemyTroops);
        float sqrtTroopsDiff = Mathf.Sqrt(troopsDiff * troopsDiff);

        if (troops > enemyTroops)
        {
            troops -= Mathf.RoundToInt(sqrtTroopsDiff * 0.1f);
            print(1);
        }
        else if (troopsDiff == 0)
        {
            troops -= troops / 10;
            print(2);
        }
        else
        {
            troops -= Mathf.RoundToInt(sqrtTroopsDiff * 0.2f);
            print(3);
        }

        print(name + " " + troops + " " + enemyTroops);

        if(troops <= 0)
        {
            Dead();
        }
    }

    private IEnumerator Battle(Unit otherUnit)
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (troops >= otherUnit.troops)
            {
                troops -= Mathf.RoundToInt(Mathf.Abs(troops - otherUnit.troops) * 0.1f);
                //print(troops);
            }
            else
            {
                troops -= Mathf.RoundToInt(Mathf.Abs(troops - otherUnit.troops) * 0.2f);
                //print("\t" + troops);
            }

            // Destroy�� ��ɾ���� ���ĸ� �� �����ؾ� �Ѵ� null ������
            if(troops <= 0)
            {
                // �� �׾��µ� �̱� ������ ����(�ڷ�ƾ) ������� ��
                Dead();
                otherUnit.isBattle = isBattle = false;
                otherUnit.StopCoroutine(otherUnit.battleCoroutine);
                StopCoroutine(battleCoroutine);
                break;
            }
        }

        //movable = true;
    }

    private void Dead()
    {
        Destroy(gameObject);
    }
}