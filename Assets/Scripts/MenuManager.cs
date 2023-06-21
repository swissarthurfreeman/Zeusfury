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

    public void PlayGame() {
        Time.timeScale = 1;
        audioSource.Stop();
        Debug.Log("PLAY MUSIC NOW");
        audioSource.clip = gameMusic;
        audioSource.Play();
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
