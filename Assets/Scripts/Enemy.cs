using System;
using UnityEngine;

[Serializable]
public class Enemy
{
    public int SpawnStartTime;
    public int SpawnEndTime;
    public int TimeBetwenSpawn;
    public float TimeUntilNextSpawn;
    public bool SpawnPositionFixed;
    [Range(0,100)]
    public int VerticalSpawnPoint;
    public GameObject EnemyGameObject;
    

    public bool IsSpawnable(float currentGameTime)
    {
        return (currentGameTime > SpawnStartTime && currentGameTime < SpawnEndTime && TimeUntilNextSpawn <= 0);
    }

    public void ResetSpawnTime()
    {
        TimeUntilNextSpawn = TimeBetwenSpawn;
    }

    public void Update()
    {
        TimeUntilNextSpawn -= Time.deltaTime;
    }
}
