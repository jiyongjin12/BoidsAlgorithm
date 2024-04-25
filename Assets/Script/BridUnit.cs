using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridUnit : MonoBehaviour
{
    public float speed;
    Boids myBoids;

    public void InitializeUnit(Boids _boids, float _speed)
    {
        myBoids = _boids;
        speed = _speed;
    }

    private void Update()
    {
        this.transform.position += this.transform.forward * speed;
    }
}
