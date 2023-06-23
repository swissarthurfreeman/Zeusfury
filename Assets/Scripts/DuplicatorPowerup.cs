using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicatorPowerup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        collected = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Obsolete]
    IEnumerator Debuff() {
        yield return new WaitForSeconds(duplicationTime);
        GameObject.Find("Zeus").GetComponent<ZeusController>().invincible = false;
        cursor.GetComponent<MeshRenderer>().enabled = false;
    }
    
    bool collected;
    public GameObject cursor;
    // called from LycaonController because collisions are completely broken
    // on duplicator for some reason
    public void ProcessCollision(GameObject other) {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        if(other.transform.CompareTag("Lycaon") && !collected) {
            collected = true;
            GameObject.Find("Zeus").GetComponent<ZeusController>().invincible = true;
            cursor = GameObject.Find("InvincibleCursor");
            cursor.GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(Debuff());
        }
    }

    public float duplicationTime = 10.0f;
}
