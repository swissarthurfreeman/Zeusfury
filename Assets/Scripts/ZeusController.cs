using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class ZeusController : MonoBehaviour
{
    public Camera ZeusCam;
    public GameObject LightningPrefab;
    public float lightningTime = 0.5f;
    public Tobii.Research.Unity.GazeTrailBase gazeTrail;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Advance() {
        //transform.position += Vector3.forward * Time.deltaTime * moveSpeed;
    }

    void Update() {
        
    }

    // Update is called once per frame
    // Raycast, mousedown, see https://learn.unity.com/tutorial/onmousedown#
    void LateUpdate()
    {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = ZeusCam.ScreenPointToRay(mousePos);

            // out passes parameter as reference, see 
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-parameter-modifier

            if(Physics.Raycast(ray, out RaycastHit hit)) {  // true if intersects a collider
                GameObject start = LightningPrefab.transform.GetChild(0).gameObject;
                start.transform.position = transform.position + new Vector3(0, 0, 10);

                GameObject end = LightningPrefab.transform.GetChild(1).gameObject;
                end.transform.position = hit.point;
                
                LightningBoltScript test = LightningPrefab.GetComponent<LightningBoltScript>();
                test.Trigger();
            }
        }     

        if(Input.GetKeyDown(KeyCode.Return)) {
            //Debug.Log(gazeTrail.latestHitPoint);
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = gazeTrail.latestHitPoint;

            GameObject start = LightningPrefab.transform.GetChild(0).gameObject;
            start.transform.position = transform.position + new Vector3(0, 0, 10);

            GameObject end = LightningPrefab.transform.GetChild(1).gameObject;
            end.transform.position = gazeTrail.latestHitPoint;
            
            LightningBoltScript test = LightningPrefab.GetComponent<LightningBoltScript>();
            test.Trigger();
        }  
    }
}