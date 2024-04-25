using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridUnit : MonoBehaviour
{
    public float speed;
    Vector3 targetVec;
    Boids myBoids;

    List<BridUnit> neighbours = new List<BridUnit>();
    [SerializeField] LayerMask boidUnitLayer;

    public void InitializeUnit(Boids _boids, float _speed)
    {
        myBoids = _boids;
        speed = _speed;
    }

    private void Update()
    {
        FindNeighbour();

        Vector3 cohesionVec = CalculateCohesionVector();

        targetVec = Vector3.Lerp(this.transform.forward, cohesionVec, Time.deltaTime);
        this.transform.rotation = Quaternion.LookRotation(targetVec);
        this.transform.position += targetVec * speed * Time.deltaTime;
    }

    private void FindNeighbour()
    {
        neighbours.Clear();

        Collider[] colls = Physics.OverlapSphere(transform.position, 20f, boidUnitLayer);
        for (int i = 0; i < colls.Length; i++)
        {
            neighbours.Add(colls[i].GetComponent<BridUnit>());
        }
    }

    public Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃 unit들의 위치 더하기
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            //이웃이 없으면 Vector3.zero 반환
            return cohesionVec;
        }

        cohesionVec /= neighbours.Count;
        cohesionVec -= transform.position;
        return cohesionVec;
    }
}
