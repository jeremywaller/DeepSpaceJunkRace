using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PowerUpBase : MonoBehaviour
{
    public int SpawnStartTime;
    public int SpawnEndTime;
    public int TimeBetwenSpawn;
    public float TimeUntilNextSpawn;
    public bool SpawnPositionFixed;
    [Range(0, 100)]
    public int VerticalSpawnPoint;
    //public GameObject PowerUpObject;

    public float MovementSpeed;
    public float DelayBeforeMovementMultiplyer;
    public float MovementSpeedMultiplyer;
    public bool MoveParent;
    private float timeSinceSpawn;
    private bool speedMultiplerApplied;
    private ParticleSystem particle;
    private GameController gameController;

    [Header("PowerUp Info")]
    [Tooltip("Amount of hull health to add when powerup is gained.")]
    public int HullAdd = 0;
    [Tooltip("Amount of shield health to add when powerup is gained.")]
    public int ShieldAdd = 0;
    [Tooltip("Amount of time ship is invincible when powerup is gained.")]
    public float InvincibilityTime = 0;
    [Tooltip("Amount of time the speed boost is active when powerup is gained.")]
    public float SpeedTime = 0;
    [Tooltip("Amount of speed to add when powerup is gained.")]
    public float SpeedMultiplier = 0;

    public void Start()
    {
        gameController = GameController.Instance;
    }

    public void Update()
    {
        TimeUntilNextSpawn -= Time.deltaTime;
    }

    private void OnEnable()
    {
        timeSinceSpawn = 0;

        var parent = transform.parent;
        if (parent != null)
        {
            particle = parent.GetComponentInChildren<ParticleSystem>();
        }

        if (particle != null)
        {
            particle.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        timeSinceSpawn += Time.deltaTime;

        if (!speedMultiplerApplied && timeSinceSpawn >= DelayBeforeMovementMultiplyer)
        {
            MovementSpeed *= MovementSpeedMultiplyer;
            speedMultiplerApplied = true;
        }

        if (MoveParent)
        {
            transform.parent.position = new Vector3(transform.parent.position.x - MovementSpeed * Time.deltaTime, transform.parent.position.y, transform.parent.position.z);
            transform.parent.Rotate(0, 0, 2);
        }
        else
        {
            transform.position = new Vector3(transform.position.x - MovementSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            transform.Rotate(0, 0, 2);
        }
    }

    private void OnDisable()
    {
        if (particle != null)
        {
            particle.gameObject.SetActive(true);
        }
    }

    public bool IsSpawnable(float currentGameTime)
    {
        return (currentGameTime > SpawnStartTime && currentGameTime < SpawnEndTime && TimeUntilNextSpawn <= 0);
    }

    public void ResetSpawnTime()
    {
        TimeUntilNextSpawn = TimeBetwenSpawn;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var playerController = collision.gameObject.GetComponent<PlayerController>();

            //add shields to player
            playerController.AddToShields(ShieldAdd);

            //add hull to player
            playerController.AddToHull(HullAdd);

            SimplePool.Despawn(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        SimplePool.Despawn(gameObject);
    }
}
