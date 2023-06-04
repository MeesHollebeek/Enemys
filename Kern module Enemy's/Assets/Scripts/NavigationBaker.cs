using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{

    public NavMeshSurface surfaces;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(UpdateNav());
        
    }

    private IEnumerator UpdateNav()
    {
        yield return new WaitForSeconds(5);

        surfaces.BuildNavMesh();

        StartCoroutine(UpdateNav());
    }

}
