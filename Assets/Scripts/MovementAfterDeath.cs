using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAfterDeath : MonoBehaviour {

    public GameObject ObjectThatDies;
    public int speedAfterDeath;

    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update ()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (ObjectThatDies != null && !ObjectThatDies.activeInHierarchy)
        {
            transform.position = new Vector3(transform.position.x - speedAfterDeath * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }
}
