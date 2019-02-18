using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public PowerUpBase[] PowerUps;

    private float top, bottom;
    private GameController gameController;

    void Start()
    {
        gameController = GameController.Instance;
        top = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y;
    }

    void FixedUpdate()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        for (int i = 0; i < PowerUps.Length; i++)
        {
            var powerUp = PowerUps[i];
            powerUp.Update();

            if (powerUp.IsSpawnable(gameController.SecondsSinceStart))
            {
                float yCoordinate;
                if (powerUp.SpawnPositionFixed)
                {
                    yCoordinate = ((top - bottom) * ((float)powerUp.VerticalSpawnPoint / 100)) + bottom;
                }
                else
                {
                    yCoordinate = UnityEngine.Random.Range(bottom, top);
                }

                var spawnPos = new Vector3(gameObject.transform.position.x, yCoordinate);

                SimplePool.Spawn(powerUp.gameObject, spawnPos, powerUp.transform.rotation);
                powerUp.ResetSpawnTime();
            }
        }
    }
}
