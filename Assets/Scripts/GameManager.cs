using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameSpeed = 10;
    private GameObject Zeus;
    private GameObject LycaonBody;
    private CharacterController LycaonCharControl;

    public List<GameObject> strips;
    public List<GameObject> ends;
    public List<GameObject> starts; 
    [SerializeField]
    private float zeusSpeed;
    private float zlim;
    private GameObject sky;

    // Start is called before the first frame update
    void Start()
    {
        Zeus = GameObject.Find("Zeus");
        LycaonBody = GameObject.Find("LycaonBody");
        LycaonCharControl = LycaonBody.GetComponent<CharacterController>();
        zeusSpeed = 0f;
        sky = GameObject.Find("Sky");
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i < strips.Count; i++) {
            strips[i].transform.position += strips[i].transform.forward * gameSpeed * Time.deltaTime;
            
            if(ends[i].transform.position.z < zlim) {
                strips[i].transform.position += ends[ mod(i - 1, ends.Count) ].transform.position - starts[i].transform.position;
            }
        }
        enforceZeusRange();
    }

    public float maxZeusLycaonDistance = 150.0f;
    public float catchUpSpeed = 10.0f;
    void enforceZeusRange() {
        Zeus.transform.position += zeusSpeed * Vector3.forward * Time.deltaTime;
        sky.transform.position += zeusSpeed * Vector3.forward * Time.deltaTime;
        zlim += zeusSpeed * Time.deltaTime;

        if( (Zeus.transform.position - LycaonBody.transform.position).magnitude >  maxZeusLycaonDistance) {
            // make Zeus catch up with Lycaon a tad
            zeusSpeed = LycaonBody.GetComponent<LycaonController>().moveSpeed - gameSpeed + catchUpSpeed;
        } else {
            zeusSpeed = 0;
        }
    }

    void LateUpdate() {
        LycaonCharControl.Move(Vector3.back * Time.deltaTime * gameSpeed);
    }

    int mod(int x, int m) {
        return (x%m + m)%m;
    }
}
