using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpelfollower : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
       
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform);
        transform.position += transform.forward * 4f * Time.deltaTime;
    }
}
