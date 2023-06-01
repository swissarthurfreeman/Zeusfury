using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripController : MonoBehaviour
{
    private float gameSpeed;
    // Start is called before the first frame update
    void Start()
    {
        gameSpeed = GameObject.Find("[GameManager]").GetComponent<GameManager>().gameSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.back * gameSpeed * Time.deltaTime;
    }
}
