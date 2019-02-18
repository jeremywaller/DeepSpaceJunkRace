using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnJunk : MonoBehaviour
{
    public List<GameObject> JunkItems;
    public float TimeBetweenSpawn;

    private float timeSinceLastSpawn;
    private float top, bottom;
    private GameController _gameController;

    // Use this for initialization
    void Start () {
        timeSinceLastSpawn = TimeBetweenSpawn;

        top = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y;
        _gameController = GameController.Instance;
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (_gameController.IsGamePaused)
        {
            return;
        }

        timeSinceLastSpawn -= Time.deltaTime;

        if (timeSinceLastSpawn <= 0)
        {
            var randY = Random.Range(bottom, top);
            var spawnPos = new Vector3(gameObject.transform.position.x, randY);
            var randomItem = JunkItems[Random.Range(0, JunkItems.Count)];

            Instantiate(randomItem, spawnPos, gameObject.transform.rotation);
            
            timeSinceLastSpawn = TimeBetweenSpawn;
        }
	}
}
