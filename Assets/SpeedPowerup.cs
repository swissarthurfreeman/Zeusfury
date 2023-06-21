using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : MonoBehaviour
{
    public float speedMultiplier = 0.3f;
    public float buffDuration = 2.0f;
    private LycaonController ctrl;
    // Start is called before the first frame update
    void Start()
    {
        ctrl = GameObject.Find("LycaonBody").GetComponent<LycaonController>();
    }

    void Update()
    {

    }

    IEnumerator Debuff() {
        Debug.Log("Start cooldown");
        yield return new WaitForSeconds(buffDuration);
        ctrl.moveSpeed -= deltaSpeed;     // remove debuff
        Destroy(gameObject);
    }

    private float deltaSpeed;
    void OnCollisionEnter(Collision other) {
        Debug.Log("Colliding");
        if(other.gameObject.CompareTag("Lycaon")) {
            GetComponent<MeshRenderer>().enabled = false;
            deltaSpeed = ctrl.moveSpeed * speedMultiplier;
            ctrl.moveSpeed += deltaSpeed; 
            StartCoroutine(Debuff());
        }
    }
}
