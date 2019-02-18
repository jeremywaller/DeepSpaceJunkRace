using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour {

    public float NextFire = 1f;
    public float FireRate = 1f;
    public bool BurstShot;
    public float DelayBetweenBursts;
    public int ShotsPerBurst;
    public int DelayUntilFirstShot;
    public GameObject Projectile;
    public bool IsEnemy;
    public bool SetProjectileAsChildObject;

    private GameController gameController;
    private float nextBurst;
    private int burstShotCount;
    private float awakeTime;
    private bool inBurstMode;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        awakeTime = Time.time;
        //SimplePool.Preload(Projectile, 20);
    }

    void Update ()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (Time.time > awakeTime + DelayUntilFirstShot && 
            (Time.time > NextFire || inBurstMode) && 
            (Input.GetButton("Fire1") || IsEnemy))
        {
            if (BurstShot && nextBurst <= 0 && burstShotCount <= ShotsPerBurst)
            {
                inBurstMode = true;
                FireProjectile();
                nextBurst = DelayBetweenBursts;
                burstShotCount++;
                if (burstShotCount == ShotsPerBurst)
                {
                    inBurstMode = false;
                    burstShotCount = 0;
                    NextFire = Time.time + FireRate;
                }
            }
            else if (!BurstShot)
            {
                FireProjectile();
                NextFire = Time.time + FireRate;
            }
        }

        nextBurst -= Time.deltaTime;
    }

    public void FireProjectile()
    {
        if (!IsEnemy && gameObject.tag != "Player") // This prevents our prefabs in the store from also firing.
        {
            return;
        }
        //var projectileInstance = SimplePool.Spawn(Projectile, transform.position, Projectile.transform.rotation);
        var projectileInstance = GameObject.Instantiate(Projectile, transform.position, Projectile.transform.rotation);
        //Debug.Log(Projectile.name + " fired by: " + gameObject.name);
        if (SetProjectileAsChildObject)
        {
            projectileInstance.transform.parent = gameObject.transform;
        }
    }
}
