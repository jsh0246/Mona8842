using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Transform target; // ĳ������ Transform

    void Update()
    {
        // ü�¹ٰ� �׻� ĳ������ ��ܿ� ��ġ�ϵ��� ����
        Vector3 offset = new Vector3(0, 2.0f, 0); // ĳ���� ���� 2 ���� �ø���
        transform.position = target.position + offset;
    }
}