using UnityEngine;

public class FirstBossController : MonoBehaviour {

    public float TimeToMaterialize;
    public float MovementSpeed;

    private bool hasMaterialized, hasInitialized;
    private bool isMovingUp;
    private float startTime;
    private float top, bottom;
    private GameObject ship;
    private Canvas healthBar;
    private SpriteRenderer shipSpriteRenderer;
    private ParticleSystem deathExplosion;
    private DialogManager dialogManager;
    private GameController gameController;
    private Damage bossHealth;

    private void Start()
    {
        Debug.Log("In the Start of FirstBossController");
        transform.position = new Vector3(5f, 0f, 0f);
        shipSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        shipSpriteRenderer.color = new Color(1f, 1f, 1f, 0); // Set ship transparent
        dialogManager = FindObjectOfType<DialogManager>();
        gameController = FindObjectOfType<GameController>();
        top = Camera.main.ViewportToWorldPoint(Vector3.zero).y + shipSpriteRenderer.bounds.extents.y;
        bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y - shipSpriteRenderer.bounds.extents.y;
        healthBar = GetComponentInChildren<Canvas>();
        healthBar.gameObject.SetActive(false);
        healthBar.sortingLayerName = "HealthBars";
        bossHealth = GetComponentInChildren<Damage>();
        bossHealth.IsInvulnerable = true;
    }

    private void Update()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (!hasMaterialized)
        {
            if (!hasInitialized)
            {
                startTime = Time.time;
                hasInitialized = true;
            }
            var alpha = Mathf.SmoothStep(0, 1f, (Time.time - startTime) / TimeToMaterialize);
            shipSpriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            if (alpha == 1f)
            {
                hasMaterialized = true;
                dialogManager.InvokeDialog("FirstBossDialog");
                healthBar.gameObject.SetActive(true);
                bossHealth.IsInvulnerable = false;
            }
        }
        else
        {
            if (isMovingUp)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - MovementSpeed * Time.deltaTime, transform.position.z);
                if (transform.position.y <= top)
                {
                    isMovingUp = false;
                }
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + MovementSpeed * Time.deltaTime, transform.position.z);
                if (transform.position.y >= bottom)
                {
                    isMovingUp = true;
                }
            }
        }
        

    }

}
