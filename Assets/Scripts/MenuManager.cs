using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public KeyCode key;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.Play();
    }

    public AudioClip menuMusic;
    public AudioClip gameMusic;
    private AudioSource audioSource;

    //public GameObject LycaonHealth;
    public GameObject LightningCoolDown;
    public GameObject DashCoolDown;
    public void PlayGame() {
        Time.timeScale = 1;
        audioSource.Stop();
        audioSource.clip = gameMusic;
        audioSource.Play();
        
        //LycaonHealth.SetActive(true);
        DashCoolDown.SetActive(true);
        LightningCoolDown.SetActive(true);
        Toggle();
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
