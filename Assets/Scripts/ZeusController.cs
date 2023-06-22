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
    private AudioSource lightningSource;

    // Start is called before the first frame update
    void Start()
    {
        Zeus = GameObject.Find("Zeus");
        LycaonBody = GameObject.Find("LycaonBody");
        sky = GameObject.Find("Sky");
        zlim = GameObject.Find("Zeus").transform.position.z - 100;
        gameSpeed = GameObject.Find("[GameManager]").GetComponent<GameManager>().gameSpeed;
        lightningSource = GetComponent<AudioSource>();
        _lightningCooldown = lightningCooldown;
    }

    void Update() {
        if(Time.timeScale == 0) return;
        // e.g. if he's waiting on cooldown before being able to shoot, don't display cursor
        Debug.Log("Hello");
        if(mana > lightningManaCost && lightningCooldown > 0)   
            Cursor.visible = false;
        else
            Cursor.visible = true;  // so if nectar has spawned or he can shoot, display pointer

        UpdateZeusRange();
        UpdateMana();
    }

    void UpdateMana() {
        lightningCooldown -= Time.deltaTime;
        mana -= Time.deltaTime;     // naturally decrease mana as Zeus advances, TODO : add mana bar
        if(mana < lightningManaCost)
            SpawnNectar();
        else
            nectarSpawned = false;  // for LateUpdate not to send lightning strike on nectar click

        if(nectarSpawned)
            CollectNectar();
    }

    void CollectNectar() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = ZeusCam.ScreenPointToRay(mousePos);
            if(Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Nectar")) {    // if we hit something that is Nectar.
                Destroy(hit.transform.gameObject);
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
            Debug.Log("Nectar Spawn");
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

    [Tooltip("Distance beyond the which Lycaon doesn't receive damage from Lightning strikes.")]
    public float lightningDamageMaxDist = 1.0f;
    public float lightningCooldown = 2.0f;
    [SerializeField]
    private float _lightningCooldown;  
    public GameObject ExplosionSmoke;
    public int impatience = 1;  // if clicks multiple times in a row, punish Zeus

    [System.Obsolete]
    void LightningStrike() {
        System.Nullable<Vector3> hitPoint = GetMouseOrEyeTrackerPoint();
        if(hitPoint.HasValue && lightningCooldown < 0) {
            lightningCooldown = _lightningCooldown;                                     // reset cooldown
            mana -= lightningManaCost;
            GameObject start = LightningPrefab.transform.GetChild(0).gameObject;        // configure lightning bolt
            start.transform.position = transform.position + new Vector3(0, 0, 10);

            GameObject end = LightningPrefab.transform.GetChild(1).gameObject;
            end.transform.position = hitPoint.Value;

            LightningBoltScript test = LightningPrefab.GetComponent<LightningBoltScript>();
            test.Trigger();     // Trigger manually triggers the lightning strike with prefab config
            
            GameObject explosion = Instantiate(ExplosionSmoke, hitPoint.Value, ExplosionSmoke.transform.rotation);
            explosion.GetComponent<ParticleSystem>().startColor = Color.yellow;
            explosion.GetComponent<ParticleSystem>().Play();

            GameObject[] lycaons = GameObject.FindGameObjectsWithTag("Lycaon");
            bool invincible = false;        // lycaon is invincible if he has clones
            foreach(GameObject lyc in lycaons) {
                if(lyc.name != "LycaonBody" && lyc.GetComponent<LycaonController>().health > 0)    // e.g. if there's a clone
                    invincible = true;
            }

            // lightning may also damage clones
            foreach(var lyc in lycaons) {
                if(!(invincible && lyc.name == "LycaonBody")) {
                    float dist = (hitPoint.Value - lyc.transform.position).magnitude;        // compute distance to Lycaon, deal damage
                    if(dist < lightningDamageMaxDist)
                        lyc.GetComponent<LycaonController>().TakeDamage(dist, lightningDamageMaxDist);
                }
            }

            lightningSource.pitch = Random.Range(1f, 3.0f);       // play lightning sound
            lightningSource.Play();
        }
    }

    // Returns the point that was either gazed at or clicked on
    // click will override the eye tracker
    // will return null if raycast didn't collide with anything
    // will return null if neither Enter (for eye tracker) or left mouse
    // were triggered
    System.Nullable<Vector3> GetMouseOrEyeTrackerPoint() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = ZeusCam.ScreenPointToRay(mousePos);
            if(Physics.Raycast(ray, out RaycastHit hit)) { // true if intersects a collider
                return hit.point;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Return)) {
            return gazeTrail.latestHitPoint;    // returns null if no collider was intersected
        }
        return null;
    }

    void LateUpdate() {
        if(!nectarSpawned && mana > lightningManaCost) {
            LightningStrike();
        }
    }
}