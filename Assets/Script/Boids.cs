using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private BridUnit bridUnitPrefab; 
    [SerializeField] public float spawnRange = 30; // ��ȯ ����
    [SerializeField] public Vector2 speedRange;
    public int bridCount;

    public float cohesionWeight = 1; // ���� ����ġ
    public float alignmentWeight = 1; // ���� ����ġ
    public float separationWeight = 1; // �и� ����ġ

    public float boundsWeight = 1; // ��� ����ġ
    public float egoWeight = 1; // ���� �̵� ����ġ

    private void Start()
    {
        for (int i = 0; i < bridCount; i++)
        {
            Vector3 randomVec = Random.insideUnitSphere; 
            randomVec *= spawnRange; 

            Quaternion radomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            BridUnit currUnit = Instantiate(bridUnitPrefab, this.transform.position + randomVec, radomRot); // ���� ���� �� �ʱ�ȭ
            currUnit.transform.SetParent(this.transform); // �θ� ����
            currUnit.InitializeUnit(this, Random.Range(speedRange.x, speedRange.y)); // �ʱ�ȭ
        }
    }
}
