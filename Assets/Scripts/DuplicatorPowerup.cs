using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicatorPowerup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        collected = false;
        rotation = Random.Range(5, 30);
    }

    float t = 1.0f;
    float rotation;
    // Update is called once per frame
    void Update()
    {
        if(dup != null) {
            if(t > 0) {
                dup.transform.Rotate(Vector3.up, rotation  * Time.deltaTime);
                t -= Time.deltaTime;
            } else {
                t = 1.0f;
                float rotation = Random.Range(-20, 20);   
            }
        }
    }

    public GameObject despawnEffect;
    private GameObject dup = null;

    [System.Obsolete]
    IEnumerator Debuff() {
        yield return new WaitForSeconds(duplicationTime);
        if(dup != null) {
            GameObject explosion = Instantiate(despawnEffect, dup.transform.position, despawnEffect.transform.rotation);
            explosion.GetComponent<ParticleSystem>().startColor = Color.red;
            explosion.GetComponent<ParticleSystem>().Play();
            dup.SetActive(false);
            yield return new WaitForSeconds(5);
            Destroy(dup);
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
    
    bool collected;
    // called from LycaonController because collisions are completely broken
    // on duplicator for some reason
    public void ProcessCollision(GameObject other) {
        GetComponent<BoxCollider>().enabled = false;
        if(other.transform.CompareTag("Lycaon") && !collected) {
            collected = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            dup = Instantiate(
                other, 
                other.transform.position, 
                other.transform.rotation
            );
            StartCoroutine(Debuff());
        }
    }

    public float duplicationTime = 10.0f;
}
