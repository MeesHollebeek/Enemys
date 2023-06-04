using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy;
    // Update is called once per frame
    void Start()
    {
        StartCoroutine(Spawning());
        
    }

    private IEnumerator Spawning()
    {
        yield return new WaitForSeconds(10);
        Instantiate(Enemy, transform.position, Quaternion.identity);

        StartCoroutine(Spawning());


    }
}
