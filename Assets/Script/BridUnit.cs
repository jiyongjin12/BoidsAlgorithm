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
    [SerializeField] float FOVAngle; // �þư�
    [SerializeField] float maxNeighbourCount; // �ִ� �̿� �� 
    [SerializeField] float neighbourDistance; // �̿� ���� �Ÿ�

    [SerializeField]
    List<BridUnit> neighbours = new List<BridUnit>();

    [SerializeField] LayerMask boidUnitLayer; // ���� �̿� Ȯ��
    [SerializeField] LayerMask obstructLeyer; // ��ֹ� Ȯ��
    // float additionalSpeed;

    Coroutine findNeighbourCoroutin; // �ֺ� �̿� ã�� �ڷ�ƾ
    Coroutine calculateEgoVectorCorutine; // ���� �̵� ��� �ڷ�ƾ

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

        Vector3 cohesionVec = CalculateCohesionVector() * myBoids.cohesionWeight; // ����
        Vector3 alignmentVec = CalculateAlignmentVector() * myBoids.alignmentWeight; // ����
        Vector3 separationVec = CalculateSeparationVector() * myBoids.separationWeight; // �и�

        Vector3 boundVec = CalculateBoundsVector() * myBoids.boundsWeight; // ���� ���� 
        Vector3 egoVecter = egoVec * myBoids.egoWeight; // ���� �̵�
        
        Vector3 obstacleVec = CalculateObstructVector() * myBoids.obstacleWeight;

        targetVec = cohesionVec + alignmentVec + separationVec + boundVec + egoVecter + obstacleVec ; // ��ǥ ���� ���

        targetVec = Vector3.Lerp(this.transform.forward, targetVec, Time.deltaTime);
        targetVec = targetVec.normalized;

        

        this.transform.rotation = Quaternion.LookRotation(targetVec);
        this.transform.position += targetVec * (speed + additionSpeed) * Time.deltaTime;
    }

    IEnumerator CalculateEgoVectorCoroutine() // ���� �̵� �ڷ�ƾ
    {
        speed = Random.Range(myBoids.speedRange.x, myBoids.speedRange.y); // ������ �ӵ� ����
        egoVec = Random.insideUnitSphere;                                 // ������ ���� �̵� ����
        yield return new WaitForSeconds(Random.Range(1f, 3f)); 
        calculateEgoVectorCorutine = StartCoroutine("CalculateEgoVectorCoroutine"); // �����
    }

    IEnumerator FindNeighborCoroutine() // �ֺ� �̿� ã�� �ڷ�ƾ
    {
        neighbours.Clear(); // ����Ʈ �ʱ�ȭ

        Collider[] colls = Physics.OverlapSphere(transform.position, neighbourDistance, boidUnitLayer); // �ֺ� ���� Ž��
        for (int i = 0; i < colls.Length; i++) 
        {
            if (Vector3.Angle(transform.forward, colls[i].transform.position - transform.position) <= FOVAngle) // �þ߰� ���� �ִ� ���� ����Ʈ�� �߰�
            {
                neighbours.Add(colls[i].GetComponent<BridUnit>());
            }
            if (i > maxNeighbourCount)
            {
                break;
            }
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        findNeighbourCoroutin = StartCoroutine("FindNeighborCoroutine"); // �����
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

    public Vector3 CalculateCohesionVector() // ����
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // �̿� unit���� ���� ��ġ ���� �̵�
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            //�̿��� ������
            return cohesionVec;
        }

        cohesionVec /= neighbours.Count;
        cohesionVec -= transform.position;
        return cohesionVec;
    }

    public Vector3 CalculateAlignmentVector() // ���� 
    {
        Vector3 alignmentVec = transform.forward;
        if (neighbours.Count > 0)
        {
            // ������ ���ϴ� ��� �������� �̵�
            for (int i = 0; i < neighbours.Count; i++)
            {
                alignmentVec += neighbours[i].transform.forward;
            }
        }
        else
        {
            // �ֺ��� ���� ������ ������ �̵�
            return alignmentVec;
        }

        alignmentVec /= neighbours.Count;
        return alignmentVec;
    }

    public Vector3 CalculateSeparationVector() // �и�
    {
        Vector3 separationVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // �̿����� ���ϴ� �������� �̵�
            for (int i = 0; i < neighbours.Count; i++)
            {
                separationVec += (transform.position - neighbours[i].transform.position);
            }
        }
        else
        {
            // �̿��� ���ٸ�
            return separationVec;
        }
        separationVec /= neighbours.Count;
        return separationVec;
    }

    private Vector3 CalculateBoundsVector() // ����
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
