using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public KeyCode key;

    // Start is called before the first frame update
    void Start() {

    }

    public void PlayGame() {
        Time.timeScale = 1;
        Toggle();
        // TODO : add code to reset Lycaon / Zeus to previous state
        // via override of reset methods in Zeus / LycaonBody, set positions
        // back and state as was. 
    }

    public void Toggle() {
        CanvasGroup g = gameObject.GetComponent<CanvasGroup>();
        g.interactable = !g.interactable;
        g.blocksRaycasts = !g.blocksRaycasts;
        g.alpha = 1 - g.alpha;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key)) {
            Toggle();
        }
    }
}
