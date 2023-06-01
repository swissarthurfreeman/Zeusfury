using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameSpeed = 10;
    private GameObject Zeus;
    private GameObject Lycaon;
    private CharacterController LycaonCharControl;

    public List<GameObject> strips;
    public List<GameObject> ends;
    public List<GameObject> starts; 
    

    // Start is called before the first frame update
    void Start()
    {
        Zeus = GameObject.Find("Zeus");
        Lycaon = GameObject.Find("LycaonBody");
        LycaonCharControl = Lycaon.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i < strips.Count; i++) {
            strips[i].transform.position += strips[i].transform.forward * gameSpeed * Time.deltaTime;
            
            if(ends[i].transform.position.z < 0) {
                strips[i].transform.position += ends[ mod(i - 1, ends.Count) ].transform.position - starts[i].transform.position;
            }
        }
    }

    void enforceZeusRange() {
        //if(Zeus.transform.position )
    }

    void LateUpdate() {
        LycaonCharControl.Move(Vector3.back * Time.deltaTime * gameSpeed);
    }

    int mod(int x, int m) {
        return (x%m + m)%m;
    }
}
