using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleBroCollective : EnemyBase {

    public float TimeToPauseForFiring;
    public float DelayBeweenChildrenShips;

    private float rightEdge;
    private float spriteSize;
    private float totalTimePaused;
    private bool isPaused;
    private EnemyBase[] children;
    private FireBullet[] childrenProjectiles;
    private float originalMovementSpeed;
    private float timeSinceLastChildFlyoff;
    private int lastChild;
    
    // Use this for initialization
	void OnEnable () {
        rightEdge = Camera.main.ViewportToWorldPoint(Vector3.one).x;
        spriteSize = GetComponentInChildren<SpriteRenderer>().bounds.extents.x;
        children = GetComponentsInChildren<EnemyBase>();
        childrenProjectiles = GetComponentsInChildren<FireBullet>();
        originalMovementSpeed = children[1].MovementSpeed;
        lastChild = children.Length;
        gameController = FindObjectOfType<GameController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (children[1] != null && children[1].gameObject.transform.position.x <= rightEdge - (spriteSize * 1.1) && !isPaused) 
        {
            isPaused = true;
            foreach (var child in children)
            {
                child.MovementSpeed = 0;
            }
            foreach (var child in childrenProjectiles)
            {
                // Now that we're paused, trigger the laser on each ship in the collective.
                child.FireProjectile();
            }
        };

        if (isPaused)
        {
            totalTimePaused += Time.deltaTime;
        }

        if (totalTimePaused >= TimeToPauseForFiring)
        {
            timeSinceLastChildFlyoff += Time.deltaTime;
            if (timeSinceLastChildFlyoff >= DelayBeweenChildrenShips && lastChild > 0)
            {
                lastChild--;
                children[lastChild].MovementSpeed = originalMovementSpeed;
                timeSinceLastChildFlyoff = 0;
            }
        }
	}
}
