using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameSpeed;
    private GameObject Zeus;
    private GameObject LycaonBody;
    private CharacterController LycaonCharControl;

    public GameObject startStrip;
    public List<GameObject> stripPrefabs;
    public Queue<GameObject> strips = new Queue<GameObject>();
    
    [SerializeField]
    private float zeusSpeed;
    private float zlim;  // gets set to behind Zeus at every update()
    private GameObject sky;
    private GameObject Map;
    private GameObject bitalinoCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Map = GameObject.Find("Map");
        Zeus = GameObject.Find("Zeus");
        LycaonBody = GameObject.Find("LycaonBody");
        LycaonCharControl = LycaonBody.GetComponent<CharacterController>();
        zeusSpeed = 0f;
        sky = GameObject.Find("Sky");
        zlim = GameObject.Find("Zeus").transform.position.z - 100;

        strips.Enqueue(startStrip);
        
        spawnStrip();
        spawnStrip();
    }

    // Update is called once per frame
    void Update()
    {
        stripRenewal();
        enforceZeusRange();
    }

    void stripRenewal() {
        GameObject firstStrip = strips.ToArray()[0];
        Bounds b = firstStrip.GetComponent<Collider>().bounds;
        if(firstStrip.transform.position.z + b.size.z / 2 < zlim) {     // if right side of strip is behind zeus
            spawnStrip();
            Destroy(strips.Dequeue());
        }
        zlim = GameObject.Find("Zeus").transform.position.z;  // to maintain appropriate despawn distance
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
