using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameSpeed = 10;
    private GameObject Zeus;
    private GameObject LycaonBody;
    private CharacterController LycaonCharControl;

    public GameObject startStrip;
    public List<GameObject> stripPrefabs;
    public Queue<GameObject> strips = new Queue<GameObject>();
    
    [SerializeField]
    private float zeusSpeed;
    private float zlim = -60;
    private GameObject sky;
    private GameObject Map;

    // Start is called before the first frame update
    void Start()
    {
        Map = GameObject.Find("Map");
        Zeus = GameObject.Find("Zeus");
        LycaonBody = GameObject.Find("LycaonBody");
        LycaonCharControl = LycaonBody.GetComponent<CharacterController>();
        zeusSpeed = 0f;
        sky = GameObject.Find("Sky");

        strips.Enqueue(startStrip);
        
        for(int i = 0; i < stripPrefabs.Count; i++) {
            GameObject newStrip = Instantiate(stripPrefabs[i]);
            Bounds precStripBounds = strips.ToArray()[i].GetComponent<BoxCollider>().bounds;
            Bounds newStripBounds = newStrip.GetComponent<BoxCollider>().bounds;        // ACCESS BOUNDS ON INSTANTIATED !
            newStrip.transform.position = strips.ToArray()[i].transform.position + Vector3.forward * (precStripBounds.size.z+newStripBounds.size.z) / 2;  // IT'S THE HALF WIDTH !
            strips.Enqueue(newStrip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        stripRenewal();
        enforceZeusRange();
    }

    void stripRenewal() {
        if(strips.ToArray()[0].transform.position.z < zlim) {
            spawnStrip();
            Destroy(strips.Dequeue());
        }
    }

    void spawnStrip() {
        int k = Random.Range(0, stripPrefabs.Count);
        GameObject newStrip = Instantiate(stripPrefabs[k]);
        Bounds precStripBounds = strips.ToArray()[strips.Count-1].GetComponent<BoxCollider>().bounds;
        Bounds newStripBounds = newStrip.GetComponent<BoxCollider>().bounds;        // ACCESS BOUNDS ON INSTANTIATED !
        newStrip.transform.position = strips.ToArray()[strips.Count-1].transform.position + Vector3.forward * (precStripBounds.size.z+newStripBounds.size.z) / 2;  // IT'S THE HALF WIDTH !
        strips.Enqueue(newStrip);
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
