using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UbhShotCtrl))]
public class PlayerBulletHellFire : MonoBehaviour
{
    public float ShotDelay;

    private GameController gameController;
    private UbhShotCtrl[] shotControllers;
    private float timeUntilNextShot;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        shotControllers = GetComponents<UbhShotCtrl>();
    }

    private void Update()
    {
        if (gameController.IsGamePaused || gameObject.tag != "Player")
        {
            return;
        }

        timeUntilNextShot -= Time.deltaTime;

        for (int i = 0; i < shotControllers.Length; i++)
        {
            if (Input.GetButton("Fire1") && !shotControllers[i].shooting && timeUntilNextShot <= 0)
            {
                shotControllers[i].StartShotRoutine();
                if (i == shotControllers.Length - 1)
                {
                    timeUntilNextShot = ShotDelay;
                }   
            }
            else
            {
                shotControllers[i].StopShotRoutine();
            }
        }

        
    }
}
