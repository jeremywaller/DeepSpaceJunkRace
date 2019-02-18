using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : Damage
{
    [Header("Enemy Values")]
    public float MovementSpeed;
    public float DelayBeforeMovementMultiplyer;
    public float MovementSpeedMultiplyer;
    public int StartingHealth;
    public bool MoveParent;

    private float timeSinceSpawn;
    private bool speedMultiplerApplied;
    private ParticleSystem particle;

    private void OnEnable()
    {
        Health = StartingHealth;
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

    void FixedUpdate ()
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
        }
        else
        {
            transform.position = new Vector3(transform.position.x - MovementSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }

    private void OnDisable()
    {
        if (Health <= 0)
        {
            if (particle != null)
            {
                particle.gameObject.SetActive(true);
            }
            ScorePoints();
        }
    }
}
