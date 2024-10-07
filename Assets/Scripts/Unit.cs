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



        // 탐색 후 아무것도 없으면 run
        // 있으면 fight
        // 아군이면 unite
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
            // 내 태그와 비교했을 때, 적이 있을때 => Fight
            else if (CompareTag("UnitBlue") && unitRed.Count > 0 || CompareTag("UnitRed") && unitBlue.Count > 0)
            {
                state = UnitState.Battle;
            }

            // 내 태그와 비교했을 때, 아군이 있을 때 => Unite
            //else if (CompareTag("UnitBlue") && unitBlue.Count > 0 || CompareTag("UnitRed") && unitRed.Count > 0)
            //{
            //    state = UnitState.Unite;
            //}

            //print(name + " is : " + unitBlue.Count + " " + unitRed.Count + " " + state);
            //print(state);

            switch (state)
            {
                case UnitState.Idle: // 움직임 간에 잠깐 여유를 줄까 말까, 주면 얼마나 줄지
                    IdleState();
                    yield return new WaitForSeconds(0.1f);
                    state = UnitState.Run;

                    break;
                case UnitState.Run: // 기본 상태
                    anim.SetBool("Attack01", false);
                    RunState(neighbours);
                    yield return new WaitForSeconds(0.6f);
                    state = UnitState.Idle;

                    break;
                case UnitState.Stand: // 사용자가 캐릭터를 눌러서 가만히 서있도록 했을때(그런 기능이 있다고 가정할 경우)
                    movable = false;

                    movable = true;
                    break;
                case UnitState.Battle:

                    // unitred unitblue 정렬하던지 뭘 먼저 공격할건지 정해야됨 지금은 그냥0번인덱스로 했음
                    if (CompareTag("UnitBlue") && unitRed.Count > 0) 
                    {
                        //3. 내가 상대로부터 데미지를 받는 방식
                        //troops -= 50;
                        //if (troops <= 0)
                        //{
                        //    counterpart.counterpart = null;
                        //}

                        // 2. 상대에게 데미지를 주는 방식
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

                    // 이런식으로 할 게 아니라 전투가 끝나면 이라는 조건이 필요할듯
                    // 지금은 waiftforsecodns(2f)는 지속적인 싸움 활용시 되는거고 전투함수를 제대로 만들어서 시작과 끝을 명확하게 받아내야할듯
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

            // 현재 움직임 중인지 판단하는 부분
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

        // tag == UnitRed일때는??
        if(other.CompareTag(tag))
        {
            Unit otherUnit = other.GetComponent<Unit>();
            if(troops >= otherUnit.troops)
            {
                // troops 와 otherUnit.troops의 값이 같은 경우 누가 죽는건가? 결과는 제대로 되는거 같은데 동시에 다 죽어야 할거같기도 하고, 정확한 매커니즘을 모르겠음
                troops += otherUnit.troops;
                // 합체 애니메이션 까지 하고 파괴
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

// 이동시 해당 방향 보기
// 유닛 머리 돌리기, 싸우는 대상 명확히 보이게 하기
// 한번에 하나씩만 싸우게 하기
// db 연동하기
// 멀티플레이
// ugui => item, boss