using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float Speed;
    public int Damage;
    public bool firedByEnemy;

    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        transform.position = new Vector2(transform.position.x + (Speed * Time.deltaTime), transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var damageScript = collision.gameObject.GetComponent<Damage>();
        var playerScript = collision.gameObject.GetComponent<PlayerController>();

        // Uncomment below to be log spammed about projectile collissions.
        //string scriptType = string.Empty;
        //if (damageScript != null)
        //{
        //    scriptType = " - A damageScript was present.";
        //}
        //else if (playerScript != null)
        //{
        //    scriptType = " - A playerScript was present.";
        //}

        //var enemydebug = firedByEnemy ? " - the projectile was fired by an enemy." : " - the projectile was not fired by an enemy.";
        //scriptType = scriptType + enemydebug;

        
        if (damageScript != null && !firedByEnemy)
        {
            damageScript.TakeDamage(Damage);
            SimplePool.Despawn(gameObject);
            //Debug.Log("Collided with: " + collision.gameObject.name + " - Despawning: " + gameObject.name + scriptType);

        }
        else if (playerScript != null && firedByEnemy)
        {
            playerScript.TakeDamage(Damage);
            SimplePool.Despawn(gameObject);
            //Debug.Log("Collided with: " + collision.gameObject.name + " - Despawning: " + gameObject.name + scriptType);

        }

    }

    void OnBecameInvisible()
    {
        SimplePool.Despawn(gameObject);
    }
}
