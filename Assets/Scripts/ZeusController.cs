using DigitalRuby.LightningBolt;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ZeusController : MonoBehaviour
{
    public Camera ZeusCam;
    public GameObject LightningPrefab;

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
        invincible = false;
    }

    void Update() {
        if(Time.timeScale == 0) return;
        // e.g. if he's waiting on cooldown before being able to shoot, don't display cursor
        if(mana > lightningManaCost && _lightningCooldown > 0)   
            Cursor.visible = false;
        else
            Cursor.visible = true;  // so if nectar has spawned or he can shoot, display pointer

        UpdateZeusRange();
        UpdateMana();

        if(_lightningCooldown < 0) {
            coolDownBar.value = 1;
        } else {
            coolDownBar.value = 1.0f - _lightningCooldown / lightningCooldown;
        }
    }

    public Slider coolDownBar;
    void UpdateMana() {
        _lightningCooldown -= Time.deltaTime;
        mana -= Time.deltaTime;     // naturally decrease mana as Zeus advances, TODO : add mana bar
        if(mana < lightningManaCost)
            SpawnNectar();
        else
            nectarSpawned = false;  // for LateUpdate not to send lightning strike on nectar click

        if(nectarSpawned)
            CollectNectar();
    }

    void CollectNectar() {
        RaycastHit? pos = GetMouseOrEyeTrackerPoint();
        if(pos.HasValue) {
            Debug.Log("Has Value passed");
            
            //Ray ray = ZeusCam.ScreenPointToRay(pos.Value);
            Debug.Log("Hit : " + pos.Value.transform.gameObject.name);
            if(pos.Value.transform.gameObject.CompareTag("Nectar")) {    // if we hit something that is Nectar.
                Destroy(pos.Value.transform.gameObject);
                mana = 100.0f;
            }
        }
    }

    public bool nectarSpawned = false;
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

    [Tooltip("Distance beyond the which Lycaon doesn't receive damage from Lightning strikes.")]
    public float lightningDamageMaxDist = 1.0f;
    public float lightningCooldown = 2.0f;
    [SerializeField]
    private float _lightningCooldown;  
    public GameObject ExplosionSmoke;
    public int impatience = 1;  // if clicks multiple times in a row, punish Zeus

    [System.Obsolete]
    void LightningStrike() {
        System.Nullable<RaycastHit> raycast = GetMouseOrEyeTrackerPoint();
        if(raycast.HasValue && _lightningCooldown < 0) {
            _lightningCooldown = lightningCooldown;                                     // reset cooldown
            mana -= lightningManaCost;
            GameObject start = LightningPrefab.transform.GetChild(0).gameObject;        // configure lightning bolt
            start.transform.position = transform.position + new Vector3(0, 0, 10);

            GameObject end = LightningPrefab.transform.GetChild(1).gameObject;
            end.transform.position = raycast.Value.point;

            LightningBoltScript test = LightningPrefab.GetComponent<LightningBoltScript>();
            test.Trigger();     // Trigger manually triggers the lightning strike with prefab config
            
            GameObject explosion = Instantiate(ExplosionSmoke, raycast.Value.point, ExplosionSmoke.transform.rotation);
            explosion.GetComponent<ParticleSystem>().startColor = Color.yellow;
            explosion.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticles(explosion));

            if(!invincible) {
                float dist = (raycast.Value.point - LycaonBody.transform.position).magnitude;        // compute distance to Lycaon, deal damage
                if(dist < lightningDamageMaxDist)
                    LycaonBody.GetComponent<LycaonController>().TakeDamage(dist, lightningDamageMaxDist);
            }
            
            lightningSource.pitch = Random.Range(1f, 3.0f);       // play lightning sound
            lightningSource.Play();
        }
    }

    public bool invincible;
	IEnumerator DestroyParticles(GameObject game) {
		yield return new WaitForSeconds(3.0f);
		Destroy(game);
	}

    public GameObject cursor;
    // Returns the point that was either gazed at or clicked on projected on world map
    // click will override the eye tracker
    // will return null if raycast didn't collide with anything
    // will return null if neither Enter (for eye tracker) or left mouse
    // were triggered
    System.Nullable<RaycastHit> GetMouseOrEyeTrackerPoint() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = ZeusCam.ScreenPointToRay(mousePos);
            if(Physics.Raycast(ray, out RaycastHit hit)) { // true if intersects a collider
                return hit;
            }
        }
        return null;
    }

    void LateUpdate() {
        if(!nectarSpawned && mana > lightningManaCost) {
            LightningStrike();
        }
    }
}