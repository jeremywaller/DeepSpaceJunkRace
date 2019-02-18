using System;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public Enemy[] Enemies;

    private float top, bottom;
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        top = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y;
    }

    void FixedUpdate()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        for (int i = 0; i < Enemies.Length; i++)
        {
            var enemy = Enemies[i];
            enemy.Update();
            if (enemy.IsSpawnable(gameController.SecondsSinceStart))
            {
                float yCoordinate;
                if (enemy.SpawnPositionFixed)
                {
                    yCoordinate = ((top - bottom) * ((float)enemy.VerticalSpawnPoint / 100)) + bottom;
                }
                else
                {
                    yCoordinate = UnityEngine.Random.Range(bottom, top);
                }

                var spawnPos = new Vector3(gameObject.transform.position.x, yCoordinate);

                SimplePool.Spawn(Enemies[i].EnemyGameObject, spawnPos, enemy.EnemyGameObject.transform.rotation);
                enemy.ResetSpawnTime();
            }
        }
    }
}
