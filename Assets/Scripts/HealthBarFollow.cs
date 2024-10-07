using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Transform target; // 캐릭터의 Transform

    void Update()
    {
        // 체력바가 항상 캐릭터의 상단에 위치하도록 설정
        Vector3 offset = new Vector3(0, 2.0f, 0); // 캐릭터 위로 2 단위 올리기
        transform.position = target.position + offset;
    }
}