using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Damage Values")]
    [Tooltip("Name that shows up in the Game Over stats screen.  Used as primary key to track kills.")]
    public string ScoreName;
    public bool IsInvulnerable;    
    public int CollissionDamage;
    [Tooltip("The number of points added when this is completely destroyed")]
    public int ScoreValue;
    public bool DestroyParentOnDeath;

    [HideInInspector]
    public int Health;

    protected GameController gameController;

    public void Start()
    {
        gameController = GameController.Instance;
    }

    public void TakeDamage(int damageDealt)
    {
        if (IsInvulnerable)
        {
            return;
        }

        Health -= damageDealt;

        if (Health <= 0)
        {
            Death();
        }
    }

    protected void ScorePoints()
    {
        if (gameController != null)
        {
            gameController.AddScore(ScoreValue);

            var enemyName = string.IsNullOrEmpty(ScoreName) ? name : ScoreName;
            gameController.AddKill(enemyName);
        }
    }

    private void Death()
    {
        //SimplePool.Despawn(gameObject);
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        //SimplePool.Despawn(gameObject);
        Destroy(gameObject);

        // Take care of empty parent object if applicable
        if (DestroyParentOnDeath)
        {
            Destroy(transform.parent.gameObject, 3f);
        }
    }
}
