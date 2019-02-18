using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    public float scrollSpeed;
    public float tileSizeZ;

    private Vector3 startPosition;
    private GameController gameController;
    private Store _store;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        gameController = FindObjectOfType<GameController>();
        _store = FindObjectOfType<Store>();
	}
	
	// Update is called once per frame
	void Update () {
        if (gameController.IsGamePaused && !gameController.IsGameOver && !_store.IsOpen)
        {
            return;
        }

        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        transform.position = startPosition + transform.up * newPosition;

	}
}
