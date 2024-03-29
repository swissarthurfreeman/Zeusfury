using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float gameSpeed;
    private GameObject Zeus;
    public GameObject startStrip;
    public List<GameObject> stripPrefabs;
    public Queue<GameObject> strips = new Queue<GameObject>();
    private float zlim;

    // Start is called before the first frame update
    void Start()
    {    
        Time.timeScale = 0;
        Zeus = GameObject.Find("Zeus");
        strips.Enqueue(startStrip);
        
        spawnStrip();
        spawnStrip();
    }

    public GameObject endArea;
    public bool endGameConditionMet = false;
    public bool lycaonDead = false;
    public bool lycaonWon = false;
    public float gameTime = 180;    // default three minutes long game.

    // Update is called once per frame
    void Update() {
        gameTime -= Time.deltaTime;
        if(gameTime < 0)
            endGameConditionMet = true;

        if(!endGameConditionMet)
            stripRenewal();
        else
            spawnEnd();

        if(lycaonDead || lycaonWon)
            StartCoroutine(ReloadGame());
    }

    public GameObject LycaonBanner;
    public GameObject ZeusBanner;

    IEnumerator ReloadGame() {
        if(lycaonDead) {
            LycaonBanner.GetComponent<TMPro.TextMeshProUGUI>().text = "You Died...";
            ZeusBanner.GetComponent<TMPro.TextMeshProUGUI>().text = "You win !";
        }

        if(lycaonWon) {
            LycaonBanner.GetComponent<TMPro.TextMeshProUGUI>().text = "You Made it !";
            ZeusBanner.GetComponent<TMPro.TextMeshProUGUI>().text = "You lost...";
        }

        LycaonBanner.SetActive(true);
        ZeusBanner.SetActive(true);

        yield return new WaitForSeconds(2);
        
        Scene s = SceneManager.GetActiveScene();	// TODO : save data here
		SceneManager.LoadScene(s.name);
    }

    private bool endSpawned = false;
    void spawnEnd() {
        if(!endSpawned) {
            endSpawned = true;
            Bounds precStripBounds = strips.ToArray()[strips.Count-1].GetComponent<BoxCollider>().bounds;
            Bounds newStripBounds = endArea.GetComponent<BoxCollider>().bounds;
            Vector3 pos = strips.ToArray()[strips.Count-1].transform.position + Vector3.forward * (precStripBounds.size.z/2 + newStripBounds.size.z + 20);
            strips.Enqueue(Instantiate(endArea, pos, endArea.transform.rotation));
        }
    }

    void stripRenewal() {
        zlim = Zeus.transform.position.z;  // to maintain appropriate despawn distance

        GameObject firstStrip = strips.ToArray()[0];
        Bounds b = firstStrip.GetComponent<Collider>().bounds;
        if(firstStrip.transform.position.z + b.size.z / 2 < zlim) {     // if right side of strip is behind zeus
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

    int mod(int x, int m) {
        return (x%m + m)%m;
    }
}
