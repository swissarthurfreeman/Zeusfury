using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicatorPowerup : MonoBehaviour
{
    private GameObject LycaonBody;
    // Start is called before the first frame update
    void Start()
    {
        LycaonBody = GameObject.Find("LycaonBody");
        rotation = Random.Range(5, 30);
    }

    float t = 1.0f;
    float rotation;
    int dir = -1;
    // Update is called once per frame
    void Update()
    {
        if(dup != null) {
            if(t > 0) {
                dup.transform.Rotate(Vector3.up, rotation  * Time.deltaTime);
                t -= Time.deltaTime;
            } else {
                t = 1.0f;
                dir *= -1;
                float rotation = Random.Range(-20, 20);   
            }
        }
    }

    public GameObject despawnEffect;
    private GameObject dup = null;

    [System.Obsolete]
    IEnumerator Debuff() {
        yield return new WaitForSeconds(duplicationTime);
        GameObject explosion = Instantiate(despawnEffect, dup.transform.position, despawnEffect.transform.rotation);
        explosion.GetComponent<ParticleSystem>().startColor = Color.red;
        explosion.GetComponent<ParticleSystem>().Play();
        dup.SetActive(false);
        yield return new WaitForSeconds(5);
        Destroy(dup);
        Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public float duplicationTime = 10.0f;
    void OnCollisionEnter(Collision other) {
        if(other.transform.CompareTag("Lycaon")) {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            dup = Instantiate(
                other.gameObject, 
                other.gameObject.transform.position, 
                other.gameObject.transform.rotation
            );
            StartCoroutine(Debuff());
        }
    }
}
