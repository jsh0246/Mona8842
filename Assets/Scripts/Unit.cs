using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public enum UnitState { Idle, Run, Stand, Battle, Dead, Unite }

    public UnitState state;
    public TextMeshPro troopsUI;
    public int troops = 100;

    private HexGrid hexGrid;
    private Animator anim;
    private Vector3Int curPos;
    private Vector3 goalPos;
    private float speed = 4f;

    [HideInInspector]
    public bool movable = false;
    [HideInInspector]
    public bool isBattle = false;
    public Coroutine battleCoroutine;
    private Unit counterpart;

    private void Awake()
    {
        hexGrid = GameObject.FindAnyObjectByType<HexGrid>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //StartCoroutine(WanderHex());

        state = UnitState.Run;
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
            //List<Vector3Int> neighbours = hexGrid.GetNeighboursForExceptObstacles(curPos);
            List<Unit> unitBlue = new();
            List<Unit> unitRed = new();
            hexGrid.WhoisMyNeighbour(curPos, ref unitBlue, ref unitRed);

            if (troops <= 0)
            {
                state = UnitState.Dead;
            }
            else if (state == UnitState.Battle && (CompareTag("UnitBlue") && unitRed.Count == 0 || CompareTag("UnitRed") && unitBlue.Count == 0))
            {
                state = UnitState.Run;
            }
            // �� �±׿� ������ ��, ���� ������ => Fight
            else if (CompareTag("UnitBlue") && unitRed.Count > 0 || CompareTag("UnitRed") && unitBlue.Count > 0)
            {
                state = UnitState.Battle;
            }

            // �� �±׿� ������ ��, �Ʊ��� ���� �� => Unite
            //else if (CompareTag("UnitBlue") && unitBlue.Count > 0 || CompareTag("UnitRed") && unitRed.Count > 0)
            //{
            //    state = UnitState.Unite;
            //}

            //print(name + " is : " + unitBlue.Count + " " + unitRed.Count + " " + state);
            //print(state);

            switch (state)
            {
                case UnitState.Idle: // ������ ���� ��� ������ �ٱ� ����, �ָ� �󸶳� ����
                    IdleState();
                    yield return new WaitForSeconds(0.1f);
                    state = UnitState.Run;

                    break;
                case UnitState.Run: // �⺻ ����
                    anim.SetBool("Attack01", false);
                    RunState(neighbours);
                    yield return new WaitForSeconds(0.6f);
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
                        //3. ���� ���κ��� �������� �޴� ���
                        //troops -= 50;
                        //if (troops <= 0)
                        //{
                        //    counterpart.counterpart = null;
                        //}

                        // 2. ��뿡�� �������� �ִ� ���
                        if (counterpart == null)
                        {
                            counterpart = unitRed[0];
                        }
                        transform.LookAt(counterpart.transform);
                        anim.SetBool("Attack01", true);
                        counterpart.troops -= 50;
                        if (counterpart.troops <= 0)
                        {
                            counterpart = null;
                        }



                        // 1. 
                        //if (unitRed[0].state != UnitState.Battle)
                        //{
                        //    GetDamage(unitRed[0].troops);
                        //}
                    } else if(CompareTag("UnitRed") && unitBlue.Count > 0) {

                        if (counterpart == null)
                        {
                            counterpart = unitBlue[0];

                        }
                        transform.LookAt(counterpart.transform);
                        anim.SetBool("Attack01", true);
                        counterpart.troops -= 50;
                        if (counterpart.troops <= 0)
                        {
                            counterpart = null;
                        }

                        //if (unitBlue[0].state != UnitState.Battle)
                        //{
                        //    GetDamage(unitBlue[0].troops);
                        //}
                    }

                    // �̷������� �� �� �ƴ϶� ������ ������ �̶�� ������ �ʿ��ҵ�
                    // ������ waiftforsecodns(2f)�� �������� �ο� Ȱ��� �Ǵ°Ű� �����Լ��� ����� ���� ���۰� ���� ��Ȯ�ϰ� �޾Ƴ����ҵ�
                    yield return new WaitForSeconds(0.1f);
                    if (troops <= 0)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(1.4f);
                    }
                    
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

        transform.rotation = Quaternion.LookRotation(goal-new Vector3(transform.position.x, 0, transform.position.z));

        movable = true;
        anim.SetBool("Run", true);
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
    }

    private void GetDamage(int enemyTroops)
    {
        int troopsDiff = Mathf.Abs(troops - enemyTroops);
        float sqrtTroopsDiff = Mathf.Sqrt(troopsDiff * troopsDiff);

        anim.SetBool("Attack01", true);

        //if (troops > enemyTroops)
        //{
        //    troops -= Mathf.RoundToInt(sqrtTroopsDiff * 2f);
        //    //print(1);
        //}
        //else if (troopsDiff == 0)
        //{
        //    troops -= troops / 10;
        //    //print(2);
        //}
        //else
        //{
        //    troops -= Mathf.RoundToInt(sqrtTroopsDiff * 4f);
        //    //print(3);
        //}

        troops -= 50;

        //print(name + " " + troops + " " + enemyTroops);

        if (troops <= 0)
        {
            state = UnitState.Dead;
            //Dead();
        }
    }

    private void Dead()
    {
        anim.SetBool("Attack01", false);
        anim.SetTrigger("Dead");
        StopAllCoroutines();
        Destroy(gameObject, 5f);
    }
}

// �̵��� �ش� ���� ����
// ���� �Ӹ� ������, �ο�� ��� ��Ȯ�� ���̰� �ϱ�
// �ѹ��� �ϳ����� �ο�� �ϱ�
// db �����ϱ�
// ��Ƽ�÷���
// ugui => item, boss