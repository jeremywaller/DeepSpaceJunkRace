using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudio : MonoBehaviour {

    public float StartingVolume = 0.33f;

    private AudioSource audioSource;
    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = StartingVolume;
    }

    // Update is called once per frame
    void Update ()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        audioSource.volume -= Time.deltaTime / 2;
    }
}
