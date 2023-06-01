using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.back * 50 * Time.deltaTime;
    }
}
