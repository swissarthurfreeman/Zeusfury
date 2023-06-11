using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class ZeusController : MonoBehaviour
{
    public Camera ZeusCam;
    public GameObject LightningPrefab;
    public Tobii.Research.Unity.GazeTrailBase gazeTrail;

    [SerializeField]
    private float zeusSpeed;

    public float maxZeusLycaonDistance = 150.0f;
    public float catchUpSpeed = 10.0f;
    
    private GameObject Zeus;
    private GameObject LycaonBody;
    private GameObject sky;
    private float zlim;  // gets set to behind Zeus at every update()
    private float gameSpeed; // copy of GameSpeed from Game manager. 
    public float mana = 100.0f;
    public float lightningManaCost = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        Zeus = GameObject.Find("Zeus");
        LycaonBody = GameObject.Find("LycaonBody");
        sky = GameObject.Find("Sky");
        zlim = GameObject.Find("Zeus").transform.position.z - 100;
        gameSpeed = GameObject.Find("[GameManager]").GetComponent<GameManager>().gameSpeed;
    }

    void Update() {
        UpdateZeusRange();
        UpdateMana();
    }

    void UpdateMana() {
        mana -= Time.deltaTime;     // naturally decrease mana as Zeus advances
        if(mana < 0)
            SpawnNectar();

        if(nectarSpawned)
            CollectNectar();
    }

    void CollectNectar() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = ZeusCam.ScreenPointToRay(mousePos);
            if(Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Nectar")) {    // if we hit something that is Nectar.
                Destroy(hit.transform.gameObject);
                nectarSpawned = false;
                mana = 100.0f;
            }
        }
    }

    private bool nectarSpawned = false;
    public GameObject Nectar; // Nectar Island Prefab
    private float minDistDepth = 40;
    private float maxDistDepth = 120;
    private float maxDistBreadth = 20;

    // Spawn a nectar cloud. Will only spawn if another nectar cloud is
    // not already present.
    void SpawnNectar() {
        if(!nectarSpawned) {
            nectarSpawned = true;
            float spawnDistDepth = Random.Range(minDistDepth, maxDistDepth);
            float spawnWidthBreadth = Random.Range(-maxDistBreadth, maxDistBreadth);
            float spawnDistHeight = -Random.Range(5, transform.position.y - 10); // we don't want to spawn Nectar out of range of Zeus

            Vector3 offset = Vector3.right * spawnWidthBreadth + Vector3.forward * spawnDistDepth + Vector3.up * spawnDistHeight;
            Instantiate(Nectar, transform.position + offset, Nectar.transform.rotation);
        }
    }
    
    void UpdateZeusRange() {
        // make Zeus catch up with Lycaon a tad
        Zeus.transform.position += zeusSpeed * Vector3.forward * Time.deltaTime;
        sky.transform.position += zeusSpeed * Vector3.forward * Time.deltaTime;
        zlim += zeusSpeed * Time.deltaTime;

        // if Lycaon is too far and Zeus still has mana, we catch up
        if( (Zeus.transform.position - LycaonBody.transform.position).magnitude >  maxZeusLycaonDistance && !nectarSpawned) {
            // ZeusSpeed = absolute Lycaon speed + catch up
            zeusSpeed = LycaonBody.GetComponent<LycaonController>().moveSpeed - gameSpeed + catchUpSpeed;
        } else {
            zeusSpeed = 0;      // if not, Zeus cannot catch up
        }
    }

    void MouseStrike() {
        mana -= lightningManaCost;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = ZeusCam.ScreenPointToRay(mousePos);

        // see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-parameter-modifier
        if(Physics.Raycast(ray, out RaycastHit hit)) {  // true if intersects a collider
            GameObject start = LightningPrefab.transform.GetChild(0).gameObject;        // configure lightning bolt
            start.transform.position = transform.position + new Vector3(0, 0, 10);

            GameObject end = LightningPrefab.transform.GetChild(1).gameObject;
            end.transform.position = hit.point;
            
            LightningBoltScript test = LightningPrefab.GetComponent<LightningBoltScript>();
            test.Trigger();     // Trigger manually triggers the lightning strike with prefab config
        }
    }

    void EyeTrackerStrike() {
        mana -= lightningManaCost;
        GameObject start = LightningPrefab.transform.GetChild(0).gameObject;
        start.transform.position = transform.position + new Vector3(0, 0, 10);

        GameObject end = LightningPrefab.transform.GetChild(1).gameObject;
        end.transform.position = gazeTrail.latestHitPoint;
        
        LightningBoltScript test = LightningPrefab.GetComponent<LightningBoltScript>();
        test.Trigger();
    }

    void LateUpdate() {
        if(mana > lightningManaCost && Input.GetMouseButtonDown(0))
            MouseStrike();

        if(mana > lightningManaCost && Input.GetKeyDown(KeyCode.Return))
            EyeTrackerStrike();
    }
}