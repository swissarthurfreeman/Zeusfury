using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance;

    // Start is called before the first frame update
    void Start() {

    }
    void Toggle() {
        CanvasGroup g = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
        g.interactable = !g.interactable;
        g.blocksRaycasts = !g.blocksRaycasts;
        g.alpha = 1 - g.alpha;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) {
            Toggle();
        }
    }
}
