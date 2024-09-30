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



        // 탐색 후 아무것도 없으면 run
        // 있으면 fight
        // 아군이면 unite
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



            // 내 태그와 비교했을 때, 적이 있을때 => Fight
            if (CompareTag("UnitBlue") && unitRed.Count > 0 || CompareTag("UnitRed") && unitBlue.Count > 0)
            {
                state = UnitState.Battle;
            }

            // 내 태그와 비교했을 때, 아군이 있을 때 => Unite
            else if (CompareTag("UnitBlue") && unitBlue.Count > 0 || CompareTag("UnitRed") && unitRed.Count > 0)
            {
                state = UnitState.Unite;
            }

            print(unitBlue.Count + " " + unitRed.Count);
            print(state);

            switch (state)
            {
                case UnitState.Idle: // 움직임 간에 잠깐 여유를 줄까 말까, 주면 얼마나 줄지
                    IdleState();
                    yield return new WaitForSeconds(0.5f);
                    state = UnitState.Run;

                    break;
                case UnitState.Run: // 기본 상태
                    RunState(neighbours);
                    yield return new WaitForSeconds(2f);
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

            // Destroy와 명령어들의 선후를 잘 생각해야 한다 null 에러남
            if (troops <= 0)
            {
                state = UnitState.Dead;
                // 난 죽었는데 이긴 놈한테 전투(코루틴) 끝내라고 해
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

        /* !! 충돌로 적인지 판별하지 않고 Hex탐색을 통해 적을 판별하자 !!*/
        // Battle을 이웃검색으로 시작하자, 충돌은 아닌듯
        // 적과의 전투
        //if(tag.CompareTo("UnitBlue") == 0 && other.CompareTag("UnitRed") || tag.CompareTo("UnitRed") == 0 && other.CompareTag("UnitBlue"))
        //{
        //    // 애니메이션과 코루틴?
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