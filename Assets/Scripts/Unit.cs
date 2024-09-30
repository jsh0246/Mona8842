using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public enum UnitState { Idle, Run, Stand, Battle, Dead, Unite }

    [HideInInspector]
    public UnitState state;
    public TextMeshPro troopsUI;
    public int troops = 100;


    private HexGrid hexGrid;
    private Rigidbody rb;
    private Animator anim;
    private Vector3Int curPos;
    private Vector3 goalPos;
    private float speed = 2f;
    private Vector3 lastPos;

    [HideInInspector]
    public bool movable = false;
    [HideInInspector]
    public bool isBattle = false;
    public Coroutine battleCoroutine;

    private void Awake()
    {
        hexGrid = GameObject.FindAnyObjectByType<HexGrid>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //StartCoroutine(WanderHex());

        state = UnitState.Run;
        lastPos = transform.position;
        StartCoroutine(StateChange());
    }

    private void Update()
    {
        troopsUI.text = troops.ToString();
    }


    private void FixedUpdate()
    {
        MoveHex();
    }

    private IEnumerator StateChange()
    {
        yield return new WaitForSeconds(1f);



        // Ž�� �� �ƹ��͵� ������ run
        // ������ fight
        // �Ʊ��̸� unite
        // 



        while (true)
        {
            yield return null;

            List<Vector3Int> neighbours = hexGrid.GetNeighboursFor(curPos);
            List<Unit> unitBlue = new();
            List<Unit> unitRed = new();
            hexGrid.WhoisMyNeighbour(curPos, ref unitBlue, ref unitRed);

            //print(rb.velocity);
            //state = UnitState.Run;

            //if (rb.velocity == Vector3.zero)
            //{
            //    state = UnitState.Idle;
            //}
            //else
            //{
            //    state = UnitState.Run;
            //}



            // �� �±׿� ������ ��, ���� ������ => Fight
            if (CompareTag("UnitBlue") && unitRed.Count > 0 || CompareTag("UnitRed") && unitBlue.Count > 0)
            {
                state = UnitState.Battle;
            }

            // �� �±׿� ������ ��, �Ʊ��� ���� �� => Unite
            else if (CompareTag("UnitBlue") && unitBlue.Count > 0 || CompareTag("UnitRed") && unitRed.Count > 0)
            {
                state = UnitState.Unite;
            }

            print(unitBlue.Count + " " + unitRed.Count);
            print(state);

            switch (state)
            {
                case UnitState.Idle: // ������ ���� ��� ������ �ٱ� ����, �ָ� �󸶳� ����
                    IdleState();
                    yield return new WaitForSeconds(0.5f);
                    state = UnitState.Run;

                    break;
                case UnitState.Run: // �⺻ ����
                    RunState(neighbours);
                    yield return new WaitForSeconds(2f);
                    state = UnitState.Idle;

                    break;
                case UnitState.Stand: // ����ڰ� ĳ���͸� ������ ������ ���ֵ��� ������(�׷� ����� �ִٰ� ������ ���)
                    movable = false;

                    movable = true;
                    break;
                case UnitState.Battle:

                    // unitred unitblue �����ϴ��� �� ���� �����Ұ��� ���ؾߵ� ������ �׳�0���ε����� ����
                    if (CompareTag("UnitBlue") && unitRed.Count > 0) 
                    {
                        print("11");
                        //StartCoroutine(BattleState(unitRed[0]));
                        GetDamage(unitRed[0].troops);
                    } else if(CompareTag("UnitRed") && unitBlue.Count > 0) {
                        print("22");
                        GetDamage(unitBlue[0].troops);
                        //StartCoroutine(BattleState(unitBlue[0]));
                    }

                    yield return new WaitForSeconds(2f);
                    anim.SetBool("Attack01", false);

                    break;
                case UnitState.Dead:
                    Dead();
                    break;
                case UnitState.Unite:
                    
                    break;
            }


        }
    }

    private void IdleState()
    {
        anim.SetBool("Run", false);

    }

    private void RunState(List<Vector3Int> neighbours)
    {
        int rDir = Random.Range(0, neighbours.Count);
        Vector3 goal = hexGrid.GetTileAt(neighbours[rDir]).transform.position;

        goalPos.x = goal.x;
        goalPos.y = transform.position.y;
        goalPos.z = goal.z;

        movable = true;
        anim.SetBool("Run", true);
    }

    private IEnumerator BattleState(Unit otherUnit)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            anim.SetBool("Attack01", true);

            print(troops + " " + otherUnit.troops);

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
            if (troops <= 0)
            {
                state = UnitState.Dead;
                // �� �׾��µ� �̱� ������ ����(�ڷ�ƾ) ������� ��
                //Dead();
                //otherUnit.isBattle = isBattle = false;
                //otherUnit.StopCoroutine(otherUnit.battleCoroutine);
                //StopCoroutine(battleCoroutine);
                break;
            }
        }

        anim.SetBool("Attack01", false);
        //state = UnitState.Run;
        //movable = true;
    }

    private void MoveHex()
    {
        if (!isBattle && movable)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.fixedDeltaTime * speed);

            // ���� ������ ������ �Ǵ��ϴ� �κ�
            //if (Vector3.Distance(lastPos, transform.position) > 0.01f)
            //{
            //    state = UnitState.Run;
            //    lastPos = transform.position;
            //}
            //else
            //{
            //    state = UnitState.Idle;
            //}
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

        anim.SetBool("Attack01", true);

        if (troops > enemyTroops)
        {
            troops -= Mathf.RoundToInt(sqrtTroopsDiff * 2f);
            //print(1);
        }
        else if (troopsDiff == 0)
        {
            troops -= troops / 10;
            //print(2);
        }
        else
        {
            troops -= Mathf.RoundToInt(sqrtTroopsDiff * 4f);
            //print(3);
        }

        //print(name + " " + troops + " " + enemyTroops);

        if (troops <= 0)
        {
            state = UnitState.Dead;
            //Dead();
            print("they all dead");
        }
    }

    private void Dead()
    {
        print("dead function");
        anim.SetTrigger("Dead");
        StopAllCoroutines();
        Destroy(gameObject, 5f);
    }
}