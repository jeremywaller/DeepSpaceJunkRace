using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringMahLazer : MonoBehaviour {

    public float FireTime;
    public float PauseBeforeFire;
    public float WarmupTime;
    public float ScaleDownTime;
    public float SecondsBetweenDamageTicks;
    public int DamageDealtPerTick;

    private float timeFired;
    private float scaleTime;
    private float scaleDownTime;
    private float warmTime;
    private bool isWarm;
    private Vector3 warmUpSize;
    private Vector3 finalSize;
    private Vector3 cooldownSize;
    private Vector2 colliderFinalSize;
    private new BoxCollider2D collider;
    private float timeUntilNextDamageTick;
    private GameController _gameController;

	void Start () {
        warmUpSize = new Vector3(-0.16f, 1);
        finalSize = new Vector3(-7.16f, 1);
        cooldownSize = new Vector3(-7.17f, 0);
        colliderFinalSize = new Vector2(8.4f, 0.8f);
        collider = gameObject.GetComponent<BoxCollider2D>();
        _gameController = GameController.Instance;
    }
	
	void Update ()
    {
        if (_gameController.IsGamePaused)
        {
            return;
        }

        CheckIfParentIsAlive();

        if (!isWarm)
        {
            warmTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, warmUpSize, warmTime);

            if (transform.localScale == warmUpSize)
            {
                isWarm = true;
            }
        }
        else
        {
            timeFired += Time.deltaTime;
            scaleTime += Time.deltaTime / WarmupTime;
            transform.localScale = Vector3.Lerp(warmUpSize, finalSize, scaleTime);
            collider.size = Vector2.Lerp(Vector2.zero, colliderFinalSize, scaleTime);

            if (timeFired >= FireTime)
            {
                scaleDownTime += Time.deltaTime / ScaleDownTime;
                transform.localScale = Vector3.Lerp(finalSize, cooldownSize, scaleDownTime);
                collider.size = Vector2.Lerp(colliderFinalSize, Vector2.zero, scaleDownTime);
                if (transform.localScale == cooldownSize)
                {
                    Destroy(gameObject);
                }
            }
        }

        // Update damage tick
        if (timeUntilNextDamageTick > 0)
        {
            timeUntilNextDamageTick -= Time.deltaTime;
        }        
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        var playerScript = collision.gameObject.GetComponent<PlayerController>();

        if (playerScript && timeUntilNextDamageTick <= 0)
        {
            playerScript.TakeDamage(DamageDealtPerTick);
            timeUntilNextDamageTick = SecondsBetweenDamageTicks;
        }
    }

    private void CheckIfParentIsAlive()
    {
        // If the ship that fired this is destroyed, then destroy the laser.
        if (!transform.parent)
        {
            SimplePool.Despawn(gameObject);
        }
    }
}
