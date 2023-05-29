using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public GameObject canvas;
    // Start is called before the first frame update
    void Awake() {
        DontDestroyOnLoad(canvas);
    }

    void Start()
    {
        CanvasGroup g = canvas.GetComponent<CanvasGroup>();
        g.interactable = true;
        g.blocksRaycasts = true;
        g.alpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) {
            
            CanvasGroup g = canvas.GetComponent<CanvasGroup>();
            g.interactable = false;
            g.blocksRaycasts = false;
            g.alpha = 0;
            SceneManager.LoadScene("Main Scene");
        }
    }
}
