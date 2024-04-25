using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private BridUnit bridUnitPrefab;
    public int bridCount;
    [SerializeField] private float spawnRange = 30;

    private void Start()
    {
        for (int i = 0; i < bridCount; i++)
        {
            Vector3 randomVec = Random.insideUnitSphere;
            randomVec *= spawnRange;

            Quaternion radomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            BridUnit currUnit = Instantiate(bridUnitPrefab, randomVec, radomRot);
            currUnit.transform.SetParent(this.transform);
        }
    }
}
