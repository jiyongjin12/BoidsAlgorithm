using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridUnit : MonoBehaviour
{
    public float speed;
    Vector3 targetVec;
    Vector3 egoVec;
    Boids myBoids;

    float additionSpeed = 0;

    [SerializeField] float obstacleDistance;
    [SerializeField] float FOVAngle; // 시아각
    [SerializeField] float maxNeighbourCount; // 최대 이웃 수 
    [SerializeField] float neighbourDistance; // 이웃 감지 거리

    [SerializeField]
    List<BridUnit> neighbours = new List<BridUnit>();

    [SerializeField] LayerMask boidUnitLayer; // 무리 이웃 확인
    [SerializeField] LayerMask obstructLeyer; // 장애물 확인
    // float additionalSpeed;

    Coroutine findNeighbourCoroutin; // 주변 이웃 찾기 코루틴
    Coroutine calculateEgoVectorCorutine; // 개인 이동 계산 코루틴

    public void InitializeUnit(Boids _boids, float _speed)
    {
        myBoids = _boids;
        speed = _speed;

        findNeighbourCoroutin = StartCoroutine("FindNeighborCoroutine");
        calculateEgoVectorCorutine = StartCoroutine("CalculateEgoVectorCoroutine");
    }

    private void Update()
    {
        if (additionSpeed > 0)
            additionSpeed -= Time.deltaTime;

        Vector3 cohesionVec = CalculateCohesionVector() * myBoids.cohesionWeight; // 응집
        Vector3 alignmentVec = CalculateAlignmentVector() * myBoids.alignmentWeight; // 정렬
        Vector3 separationVec = CalculateSeparationVector() * myBoids.separationWeight; // 분리

        Vector3 boundVec = CalculateBoundsVector() * myBoids.boundsWeight; // 범위 지정 
        Vector3 egoVecter = egoVec * myBoids.egoWeight; // 개인 이동
        
        Vector3 obstacleVec = CalculateObstructVector() * myBoids.obstacleWeight;

        targetVec = cohesionVec + alignmentVec + separationVec + boundVec + egoVecter + obstacleVec ; // 목표 백터 계산

        targetVec = Vector3.Lerp(this.transform.forward, targetVec, Time.deltaTime);
        targetVec = targetVec.normalized;

        

        this.transform.rotation = Quaternion.LookRotation(targetVec);
        this.transform.position += targetVec * (speed + additionSpeed) * Time.deltaTime;
    }

    IEnumerator CalculateEgoVectorCoroutine() // 개인 이동 코루틴
    {
        speed = Random.Range(myBoids.speedRange.x, myBoids.speedRange.y); // 무작위 속도 설정
        egoVec = Random.insideUnitSphere;                                 // 무작위 개인 이동 설정
        yield return new WaitForSeconds(Random.Range(1f, 3f)); 
        calculateEgoVectorCorutine = StartCoroutine("CalculateEgoVectorCoroutine"); // 재시작
    }

    IEnumerator FindNeighborCoroutine() // 주변 이웃 찾기 코루틴
    {
        neighbours.Clear(); // 리스트 초기화

        Collider[] colls = Physics.OverlapSphere(transform.position, neighbourDistance, boidUnitLayer); // 주변 유닛 탐색
        for (int i = 0; i < colls.Length; i++) 
        {
            if (Vector3.Angle(transform.forward, colls[i].transform.position - transform.position) <= FOVAngle) // 시야각 내에 있는 유닛 리스트에 추가
            {
                neighbours.Add(colls[i].GetComponent<BridUnit>());
            }
            if (i > maxNeighbourCount)
            {
                break;
            }
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        findNeighbourCoroutin = StartCoroutine("FindNeighborCoroutine"); // 재시작
    }

    //private void FindNeighbour()
    //{
    //    neighbours.Clear();

    //    Collider[] colls = Physics.OverlapSphere(transform.position, 20f, boidUnitLayer);
    //    for (int i = 0; i < colls.Length; i++)
    //    {
    //        neighbours.Add(colls[i].GetComponent<BridUnit>());
    //    }
    //}

    public Vector3 CalculateCohesionVector() // 응집
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃 unit들의 중점 위치 구해 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            //이웃이 없으면
            return cohesionVec;
        }

        cohesionVec /= neighbours.Count;
        cohesionVec -= transform.position;
        return cohesionVec;
    }

    public Vector3 CalculateAlignmentVector() // 정렬 
    {
        Vector3 alignmentVec = transform.forward;
        if (neighbours.Count > 0)
        {
            // 새들이 향하는 평균 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                alignmentVec += neighbours[i].transform.forward;
            }
        }
        else
        {
            // 주변에 새가 없으면 앞으로 이동
            return alignmentVec;
        }

        alignmentVec /= neighbours.Count;
        return alignmentVec;
    }

    public Vector3 CalculateSeparationVector() // 분리
    {
        Vector3 separationVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃들을 피하는 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                separationVec += (transform.position - neighbours[i].transform.position);
            }
        }
        else
        {
            // 이웃이 없다면
            return separationVec;
        }
        separationVec /= neighbours.Count;
        return separationVec;
    }

    private Vector3 CalculateBoundsVector() // 범위
    {
        Vector3 offsetToCenter = myBoids.W.transform.position - transform.position;
        return offsetToCenter.magnitude >= myBoids.spawnRange ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstructVector()
    {
        Vector3 ObstructVec = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleDistance, obstructLeyer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.blue);
            ObstructVec = hit.normal;
            additionSpeed = 10;
        }

        return ObstructVec;
    }
}
