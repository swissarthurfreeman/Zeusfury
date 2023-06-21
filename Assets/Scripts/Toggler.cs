using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Toggle() {
        CanvasGroup g = gameObject.GetComponent<CanvasGroup>();
        g.interactable = !g.interactable;
        g.blocksRaycasts = !g.blocksRaycasts;
        g.alpha = 1 - g.alpha;
    }

    public KeyCode key;
    void Update()
    {
        if(Input.GetKeyDown(key)) {
            Toggle();
        }
    }
}
