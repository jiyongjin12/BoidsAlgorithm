using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private BridUnit bridUnitPrefab; 
    [SerializeField] public float spawnRange = 30; // 소환 범위
    [SerializeField] public Vector2 speedRange;
    public int bridCount;

    public float cohesionWeight = 1; // 응집 가중치
    public float alignmentWeight = 1; // 정렬 가중치
    public float separationWeight = 1; // 분리 가중치

    public float boundsWeight = 1; // 경계 가중치
    public float egoWeight = 1; // 개인 이동 가중치

    private void Start()
    {
        for (int i = 0; i < bridCount; i++)
        {
            Vector3 randomVec = Random.insideUnitSphere; 
            randomVec *= spawnRange; 

            Quaternion radomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            BridUnit currUnit = Instantiate(bridUnitPrefab, this.transform.position + randomVec, radomRot); // 유닛 생성 후 초기화
            currUnit.transform.SetParent(this.transform); // 부모 설정
            currUnit.InitializeUnit(this, Random.Range(speedRange.x, speedRange.y)); // 초기화
        }
    }
}
